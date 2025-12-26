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

        // Deduplicate in-memory by (PhoneNumber, CallDate)
        List<CallEntity> uniqueEntities =
        [
            .. entities
            .GroupBy(e => (e.PhoneNumber, e.CallDate))
            .Select(g => g.Last())
        ];

        const int batchSize = 50;

        try
        {
            // SQLite performance pragmas
            await _context.Database.ExecuteSqlRawAsync("PRAGMA journal_mode = WAL;");
            await _context.Database.ExecuteSqlRawAsync("PRAGMA synchronous = NORMAL;");
            await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            foreach (var batch in uniqueEntities.Chunk(batchSize))
            {
                StringBuilder sql = new();
                sql.Append(
                    "INSERT INTO CallEntities " +
                    "(PhoneNumber, CallDate, UnixCallDate, Note, Source, Location, Duration, Billable) VALUES ");

                for (int i = 0; i < batch.Length; i++)
                {
                    CallEntity e = batch[i];

                    // Escape strings once
                    string note = e.Note?.Replace("'", "''") ?? "";
                    string source = e.Source?.Replace("'", "''") ?? "";
                    string location = e.Location?.Replace("'", "''") ?? "";

                    sql.Append(
                        $"({e.PhoneNumber}, " +
                        $"'{e.CallDate:yyyy-MM-dd HH:mm:ss}', " +
                        $"{e.UnixCallDate}, " +
                        $"'{note}', " +
                        $"'{source}', " +
                        $"'{location}', " +
                        $"{e.Duration}, " +
                        $"{(e.Billable ? 1 : 0)})");

                    if (i < batch.Length - 1)
                        sql.Append(", ");
                }

                sql.Append(
                    " ON CONFLICT(PhoneNumber, CallDate) DO UPDATE SET " +
                    "UnixCallDate = excluded.UnixCallDate, " +
                    "Note = excluded.Note, " +
                    "Source = excluded.Source, " +
                    "Location = excluded.Location, " +
                    "Duration = excluded.Duration, " +
                    "Billable = excluded.Billable;"
                );

                await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            }

            await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
            await transaction.CommitAsync();

            _logger.LogDebug("CallEntity upsert completed: Total={Total}, Unique={Unique}", entities.Count, uniqueEntities.Count);

            return Result.Success(uniqueEntities);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<CallEntity>>(ex.Message);
        }
    }

}
