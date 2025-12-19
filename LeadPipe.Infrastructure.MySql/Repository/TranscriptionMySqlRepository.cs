using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Interfaces.Repository.MySql;
using LeadPipe.Infrastructure.MySql.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.MySql.Repository;

public class TranscriptionMySqlRepository(MySqlContext context) : ITranscriptionMySqlRepository
{
    private readonly DbSet<TranscriptionMySqlEntity> _set = context.Set<TranscriptionMySqlEntity>();

    public async Task<Result<List<TranscriptionMySqlEntity>>> FindAsync(Expression<Func<TranscriptionMySqlEntity, bool>> predicate)
    {
        try
        {
            var list = await _set.AsNoTracking().Where(predicate).ToListAsync();
            return Result.Success(list);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<TranscriptionMySqlEntity>>(ex.Message);
        }
    }

    public async Task<Result<TranscriptionMySqlEntity>> GetByIdAsync(long callId)
    {
        var found = await _set.AsNoTracking().SingleOrDefaultAsync(t => t.call_id == callId);

        return found is null
            ? Result.Failure<TranscriptionMySqlEntity>("Not found")
            : Result.Success(found);
    }
}
