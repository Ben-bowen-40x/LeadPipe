using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public abstract class PlumbingContextRepository<T, T2>(PlumbingContext context, ILogger<T2> logger)
    : IRepository<T> where T : class, IEntity
{
    protected readonly PlumbingContext _context = context;
    protected readonly DbSet<T> _set = context.Set<T>();
    internal readonly ILogger<T2> _logger = logger;
    public async Task<Result<List<T>>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            var list = await _set.Where(predicate).ToListAsync();
            return Result.Success(list);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<T>>(ex.ToString());
        }
    }

    public virtual async Task<Result<List<T>>> GetAllAsync()
    {
        try
        {
            List<T> list = await _set.ToListAsync();
            return Result.Success(list);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<T>>($"Failed to get entities: {ex}");
        }
    }
    public virtual async Task<Result<List<T>>> GetAllWithDetailsAsync()
    {
        throw new NotSupportedException(
            $"{typeof(T2).Name} must implement a GetAllWithDetailsAsync.");
    }

    public virtual async Task<Result<List<T>>> UpsertRangeAsync(List<T> entities)
    {
        throw new NotSupportedException(
        $"{typeof(T2).Name} must implement a SQLite-native UpsertRangeAsync.");
    }
}
