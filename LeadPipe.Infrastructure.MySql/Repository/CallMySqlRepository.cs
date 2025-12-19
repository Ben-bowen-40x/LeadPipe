using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Interfaces.Repository.MySql;
using LeadPipe.Infrastructure.MySql.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.MySql.Repository;

public class CallMySqlRepository(MySqlContext context) : ICallMySqlRepository
{
    private readonly DbSet<CallMySqlEntity> _set = context.Set<CallMySqlEntity>();

    public async Task<Result<List<CallMySqlEntity>>> FindAsync(Expression<Func<CallMySqlEntity, bool>> predicate, bool includeDetails = true)
    {
        try
        {
            IQueryable<CallMySqlEntity> query = _set.AsNoTracking();

            if (includeDetails)
            {
                query = query
                    .Include(c => c.summaries)
                    .Include(c => c.transcriptions);
            }

            var list = await query.Where(predicate).ToListAsync();
            return Result.Success(list);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<CallMySqlEntity>>(ex.Message);
        }
    }

    public async Task<Result<CallMySqlEntity>> GetByIdAsync(long id, bool includeDetails = true)
    {
        IQueryable<CallMySqlEntity> query = _set.AsNoTracking();

        if (includeDetails)
        {
            query = query
                .Include(c => c.summaries)
                .Include(c => c.transcriptions);
        }

        var found = await query.SingleOrDefaultAsync(c => c.call_id == id);

        return found is null
            ? Result.Failure<CallMySqlEntity>($"Entity with id {id} was not found")
            : Result.Success(found);
    }
}
