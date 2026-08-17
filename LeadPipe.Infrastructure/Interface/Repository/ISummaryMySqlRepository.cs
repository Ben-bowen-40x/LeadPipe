using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interface.Repository;

public interface ISummaryMySqlRepository
{
    Task<Result<List<SummaryMySqlEntity>>> FindAsync(Expression<Func<SummaryMySqlEntity, bool>> predicate);
    Task<Result<SummaryMySqlEntity>> GetByIdAsync(long callId);
}
