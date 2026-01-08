using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public sealed class CallRepository(PlumbingContext context, ILogger<CallRepository> logger)
    : PlumbingContextRepository<CallEntity, CallRepository>(context, logger), ICallRepository
{
    public override async Task<Result<List<CallEntity>>> UpsertRangeAsync(List<CallEntity> entities)
    {
        if (entities.Count == 0)
            return Result.Success(new List<CallEntity>());

        // Deduplicate
        List<CallEntity> uniqueEntities =
        [
            .. entities
            .GroupBy(e => (e.PhoneNumber, e.CallDate))
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
            CREATE TEMP TABLE IF NOT EXISTS temp_calls (
                PhoneNumber INTEGER NOT NULL,
                CallDate TEXT NOT NULL,
                UnixCallDate INTEGER NOT NULL,
                Note TEXT,
                Source TEXT,
                Location TEXT,
                Duration INTEGER,
                Billable INTEGER,
                PRIMARY KEY (PhoneNumber, CallDate)
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

                    // Gradually scale back up after success
                    if (batchSize < 200)
                        batchSize = Math.Min(batchSize * 2, 200);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "{Entity} batch insert failed (size={BatchSize}). Reducing batch size.",
                        nameof(CallEntity),
                        batchSize);

                    if (batchSize == minBatchSize)
                    {
                        var row = batch[0];

                        _logger.LogError(
                            "Row insert failed: Phone={Phone}, CallDate={CallDate}, Note={Note}, Source={Source}, Location={Location}",
                            row.PhoneNumber,
                            row.CallDate,
                            row.Note,
                            row.Source,
                            row.Location);

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
            UPDATE CallEntities
            SET
                UnixCallDate = (
                    SELECT t.UnixCallDate
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                ),
                Note = (
                    SELECT t.Note
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                ),
                Source = (
                    SELECT t.Source
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                ),
                Location = (
                    SELECT t.Location
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                ),
                Duration = (
                    SELECT t.Duration
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                ),
                Billable = (
                    SELECT t.Billable
                    FROM temp_calls t
                    WHERE t.PhoneNumber = CallEntities.PhoneNumber
                      AND t.CallDate = CallEntities.CallDate
                )
            WHERE EXISTS (
                SELECT 1
                FROM temp_calls t
                WHERE t.PhoneNumber = CallEntities.PhoneNumber
                  AND t.CallDate = CallEntities.CallDate
            );
        """);

            // ---- Phase 2: INSERT missing rows ----
            int inserted = await _context.Database.ExecuteSqlRawAsync("""
            INSERT INTO CallEntities
                (PhoneNumber, CallDate, UnixCallDate, Note, Source, Location, Duration, Billable)
            SELECT
                t.PhoneNumber,
                t.CallDate,
                t.UnixCallDate,
                t.Note,
                t.Source,
                t.Location,
                t.Duration,
                t.Billable
            FROM temp_calls t
            WHERE NOT EXISTS (
                SELECT 1
                FROM CallEntities c
                WHERE c.PhoneNumber = t.PhoneNumber
                  AND c.CallDate = t.CallDate
            );
        """);

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM temp_calls;");
            await transaction.CommitAsync();

            _logger.LogInformation(
                "CallEntity upsert complete: Incoming={Incoming}, Unique={Unique}, Staged={Staged}, Updated={Updated}, Inserted={Inserted}, Skipped={Skipped}",
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
            _logger.LogError(ex, "CallEntity upsert failed");
            return Result.Failure<List<CallEntity>>(ex.ToString());
        }

        // ---- Local helper ----
        void InsertBatch(List<CallEntity> batch)
        {
            var sql = new StringBuilder();
            sql.Append("INSERT INTO temp_calls VALUES ");

            for (int i = 0; i < batch.Count; i++)
            {
                var e = batch[i];

                sql.Append($"""
                (
                    {e.PhoneNumber},
                    '{e.CallDate:yyyy-MM-dd HH:mm:ss}',
                    {e.UnixCallDate},
                    '{Clean(e.Note)}',
                    '{Clean(e.Source)}',
                    '{Clean(e.Location)}',
                    {e.Duration},
                    {(e.Billable ? 1 : 0)}
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

        // SQLite cannot handle embedded nulls
        value = value.Replace("\0", string.Empty);

        // Escape single quotes for raw SQL
        return value.Replace("'", "''");
    }
}