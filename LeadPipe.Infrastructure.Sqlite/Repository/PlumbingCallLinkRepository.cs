using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public sealed class PlumbingCallLinkRepository(PlumbingContext context, ILogger<PlumbingCallLinkRepository> logger)
    : PlumbingContextRepository<PlumbingCallLink, PlumbingCallLinkRepository>(context, logger), IPlumbingCallLinkRepository
{
    public override async Task<Result<List<PlumbingCallLink>>> UpsertRangeAsync(List<PlumbingCallLink> entities)
    {
        if (entities.Count == 0)
            return Result.Success(new List<PlumbingCallLink>());

        // Deduplicate in-memory by (PlumbingId, CallId)
        List<PlumbingCallLink> uniqueEntities = [.. entities
            .GroupBy(e => (e.PlumbingId, e.CallId))
            .Select(g => g.Last())];

        const int parametersPerRow = 2;
        const int batchSize = 999 / parametersPerRow; // Max rows per batch
        List<List<PlumbingCallLink>> batches = [.. uniqueEntities
            .Select((e, i) => new { e, i })
            .GroupBy(x => x.i / batchSize)
            .Select(g => g.Select(x => x.e).ToList())];

        try
        {
            await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            foreach (List<PlumbingCallLink>? batch in batches)
            {
                StringBuilder sqlBuilder = new();
                sqlBuilder.Append(
                    "INSERT INTO PlumbingCallLinks " +
                    "(PlumbingId, CallId) VALUES ");

                List<SqliteParameter> parameters = [];
                for (int i = 0; i < batch.Count; i++)
                {
                    PlumbingCallLink e = batch[i];
                    sqlBuilder.Append($"(@PlumbingId{i}, @CallId{i})");
                    if (i < batch.Count - 1)
                        sqlBuilder.Append(", ");

                    parameters.AddRange(
                    [
                        new SqliteParameter($"@PlumbingId{i}", e.PlumbingId),
                        new SqliteParameter($"@CallId{i}", e.CallId)
                    ]);
                }

                sqlBuilder.AppendLine(
                    " ON CONFLICT(PlumbingId, CallId) DO UPDATE SET " +
                    "PlumbingId=excluded.PlumbingId, " +
                    "CallId=excluded.CallId;"); // Currently only keys, but keeps conflict-safe

                await _context.Database.ExecuteSqlRawAsync(sqlBuilder.ToString(), parameters);
            }

            await transaction.CommitAsync();

            _logger.LogDebug("PlumbingCallLink upsert completed: Total={Total}, Unique={Unique}", entities.Count, uniqueEntities.Count);

            return Result.Success(uniqueEntities);
        }
        catch (Exception ex) { return Result.Failure<List<PlumbingCallLink>>(ex.Message); }
    }
}
