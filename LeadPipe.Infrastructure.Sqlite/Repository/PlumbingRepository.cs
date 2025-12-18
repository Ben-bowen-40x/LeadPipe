using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public class PlumbingRepository(
    PlumbingContext context, 
    ILogger<PlumbingRepository> logger
    ) : PlumbingContextRepository<PlumbingEntity>(context), IPlumbingRepository
{
    private readonly ILogger<PlumbingRepository> _logger = logger;
    public async Task<Result<List<PlumbingEntity>>> GetAllAsync(Source source)
    {
        try
        {
            List<PlumbingEntity> plumbing = await _set
                .Where(p => p.Source == source)
                .ToListAsync();
            return Result.Success(plumbing);
        }
        catch (Exception ex) { return Result.Failure<List<PlumbingEntity>>(ex.Message); }
    }
    public override async Task<Result<List<PlumbingEntity>>> AddRangeAsync(List<PlumbingEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Failure<List<PlumbingEntity>>("No entities provided.");

        try
        {
            // Extract numbers
            HashSet<long> phoneNumbers = [.. entities.Select(e => e.PhoneNumber)];

            // Query
            var existing = await _set
                .Where(p => phoneNumbers.Contains(p.PhoneNumber))
                .Select(p => new { p.PhoneNumber, p.Source })
                .ToListAsync();

            // Now finish the composite match in memory
            HashSet<(long PhoneNumber, Source Source)> existingSet = [.. existing.Select(x => (x.PhoneNumber, x.Source))];

            List<PlumbingEntity> toInsert = [.. entities.Where(e => !existingSet.Contains((e.PhoneNumber, e.Source)))];

            if (toInsert.Count == 0)
                return Result.Success(new List<PlumbingEntity>());

            await _set.AddRangeAsync(toInsert);
            await _context.SaveChangesAsync();

            _logger.LogDebug(
                "Plumbing bulk insert: {Inserted}/{Total}",
                toInsert.Count,
                entities.Count);

            return Result.Success(toInsert);
        }
        catch (Exception ex) { return Result.Failure<List<PlumbingEntity>>($"Failed to save Plumbing entities: {ex.Message}"); }
    }

}
/*Microsoft.Data.Sqlite.SqliteException
  HResult=0x80004005
  Message=SQLite Error 14: 'unable to open database file'.
  Source=Microsoft.Data.Sqlite
  StackTrace:
   at Microsoft.Data.Sqlite.SqliteException.ThrowExceptionForRC(Int32 rc, sqlite3 db)
   at Microsoft.Data.Sqlite.SqliteConnectionInternal..ctor(SqliteConnectionStringBuilder connectionOptions, SqliteConnectionPool pool)
   at Microsoft.Data.Sqlite.SqliteConnectionPool.GetConnection()
   at Microsoft.Data.Sqlite.SqliteConnectionFactory.GetConnection(SqliteConnection outerConnection)
   at Microsoft.Data.Sqlite.SqliteConnection.Open()
   at System.Data.Common.DbConnection.OpenAsync(CancellationToken cancellationToken)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.<OpenInternalAsync>d__70.MoveNext()
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.<OpenInternalAsync>d__70.MoveNext()
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.<OpenAsync>d__66.MoveNext()
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.<ExecuteReaderAsync>d__18.MoveNext()
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.<InitializeReaderAsync>d__21.MoveNext()
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.<MoveNextAsync>d__20.MoveNext()
   at System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable`1.ConfiguredValueTaskAwaiter.GetResult()
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.<ToListAsync>d__67`1.MoveNext()
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.<ToListAsync>d__67`1.MoveNext()
   at LeadPipe.Infrastructure.Sqlite.Repository.PlumbingRepository.<AddRangeAsync>d__3.MoveNext() in C:\Users\benjamin.bowen\Repos\LeadPipe\LeadPipe.Infrastructure.Sqlite\Repository\PlumbingRepository.cs:line 39

  This exception was originally thrown at this call stack:
    [External Code]
    LeadPipe.Infrastructure.Sqlite.Repository.PlumbingRepository.AddRangeAsync(System.Collections.Generic.List<LeadPipe.Infrastructure.Entity.Sqlite.PlumbingEntity>) in PlumbingRepository.cs*/