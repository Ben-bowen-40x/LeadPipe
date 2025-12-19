using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.MySql;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interfaces.Repository.MySql;

public interface ISubMySqlRepository
{
    Task<Result<List<SubMySqlEntity>>> FindAsync(Expression<Func<SubMySqlEntity, bool>> predicate, bool includeCustomer);
    Task<Result<SubMySqlEntity>> GetByIdAsync(int id, bool includeCustomer);
}
