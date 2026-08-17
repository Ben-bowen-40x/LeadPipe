using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interface.Repository;

public interface ICustardMySqlRepository
{
    Task<Result<List<CustardMySqlEntity>>> FindAsync(Expression<Func<CustardMySqlEntity, bool>> predicate, bool includeSubscriptions = true);
    Task<Result<CustardMySqlEntity>> GetByIdAsync(int id, bool includeSubscriptions = true);
}
