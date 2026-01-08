using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public sealed class CornRepository(
    PlumbingContext context,
    ILogger<CornRepository> logger)
    : PlumbingContextRepository<CornEntity, CornRepository>(context, logger),
      ICornRepository
{
    public override async Task<Result<List<CornEntity>>> UpsertRangeAsync(
        List<CornEntity> entities)
    {
        if (entities.Count == 0)
            return Result.Success(new List<CornEntity>());

        // Deduplicate
        List<CornEntity> uniqueEntities =
        [
            .. entities
                .OrderByDescending(e => e.Date)
                .GroupBy(e => (e.PhoneNumber, e.Source))
                .Select(g => g.Last())
        ];

        int batchSize = 200;
        const int minBatchSize = 1;

        try
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            // Temp table (connection-scoped)
            await _context.Database.ExecuteSqlRawAsync("""
                CREATE TEMP TABLE IF NOT EXISTS temp_corn (
                    PhoneNumber TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    UnixDate INTEGER NOT NULL,
                    Payload TEXT NOT NULL,
                    MetaData TEXT NOT NULL,
                    Source TEXT NOT NULL,
                    PRIMARY KEY (PhoneNumber, Date)
                ) WITHOUT ROWID;
            """);

            int index = 0;
            int stagedCount = 0;
            int skipped = 0;

            while (index < uniqueEntities.Count)
            {
                int take = Math.Min(batchSize, uniqueEntities.Count - index);
                var batch = uniqueEntities.GetRange(index, take);

                try
                {
                    InsertBatch(batch);
                    stagedCount += batch.Count;
                    index += take;

                    if (batchSize < 200)
                        batchSize = Math.Min(batchSize * 2, 200);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "{Entity} batch insert failed (size={BatchSize}). Reducing batch size.",
                        nameof(CornEntity),
                        batchSize);

                    if (batchSize == minBatchSize)
                    {
                        var row = batch[0];

                        _logger.LogError(
                            "{Entity} row insert failed: Phone={Phone}, Source={Source}",
                            nameof(CornEntity),
                            row.PhoneNumber,
                            row.Source);

                        index++;
                        skipped++;
                        batchSize = 100;
                    }
                    else
                    {
                        batchSize = Math.Max(minBatchSize, batchSize / 2);
                    }
                }
            }

            // ---- Phase 1: UPDATE existing rows ----
            int updated = await _context.Database.ExecuteSqlRawAsync("""
                UPDATE CornEntities
                SET
                    MetaData = (
                        SELECT t.MetaData
                        FROM temp_corn t
                        WHERE t.PhoneNumber = CornEntities.PhoneNumber
                          AND t.Source = CornEntities.Source
                    ),
                    Payload = (
                        SELECT t.Payload
                        FROM temp_corn t
                        WHERE t.PhoneNumber = CornEntities.PhoneNumber
                          AND t.Source = CornEntities.Source
                    ),
                    Date = (
                        SELECT t.Date
                        FROM temp_corn t
                        WHERE t.PhoneNumber = CornEntities.PhoneNumber
                          AND t.Source = CornEntities.Source
                    ),
                    UnixDate = (
                        SELECT t.UnixDate
                        FROM temp_corn t
                        WHERE t.PhoneNumber = CornEntities.PhoneNumber
                          AND t.Source = CornEntities.Source
                    )
                WHERE EXISTS (
                    SELECT 1
                    FROM temp_corn t
                    WHERE t.PhoneNumber = CornEntities.PhoneNumber
                      AND t.Source = CornEntities.Source
                );
            """);

            // ---- Phase 2: INSERT missing rows ----
            int inserted = await _context.Database.ExecuteSqlRawAsync("""
                INSERT INTO CornEntities
                    (PhoneNumber, Date, UnixDate, Payload, MetaData, Source)
                SELECT
                    t.PhoneNumber,
                    t.Date,
                    t.UnixDate
                    t.Payload,
                    t.MetaData,
                    t.Source,
                FROM temp_corn t
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM CornEntities c
                    WHERE c.PhoneNumber = t.PhoneNumber
                      AND c.Source = t.Source
                );
            """);

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM temp_corn;");
            await transaction.CommitAsync();

            _logger.LogInformation(
                "{Entity} upsert complete: Incoming={Incoming}, Unique={Unique}, Staged={Staged}, Updated={Updated}, Inserted={Inserted}, Skipped={Skipped}",
                nameof(CornEntity),
                entities.Count,
                uniqueEntities.Count,
                stagedCount,
                updated,
                inserted,
                skipped);

            return Result.Success(uniqueEntities);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Entity} upsert failed", 
                nameof(CornEntity));
            return Result.Failure<List<CornEntity>>(ex.ToString());
        }

        // ---- Local helper ----
        void InsertBatch(List<CornEntity> batch)
        {
            var sql = new StringBuilder();
            sql.Append("INSERT INTO temp_corn VALUES ");

            for (int i = 0; i < batch.Count; i++)
            {
                var e = batch[i];

                sql.Append($"""
                (
                    '{e.PhoneNumber}',
                    '{e.Date:yyyy-MM-dd HH:mm:ss}',
                    {e.UnixDate},
                    '{Clean(e.Payload)}',
                    '{Clean(e.MetaData)}',
                    '{e.Source}'
                )
                """);

                if (i < batch.Count - 1)
                    sql.Append(", ");
            }

            sql.Append(';');
            _context.Database.ExecuteSqlRaw(sql.ToString());
        }
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        value = value.Replace("\0", string.Empty);
        return value.Replace("'", "''");
    }
}
