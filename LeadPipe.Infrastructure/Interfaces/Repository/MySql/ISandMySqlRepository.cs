using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.MySql;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interfaces.Repository.MySql;

public interface ISandMySqlRepository
{
    Task<Result<List<SandMySqlEntity>>> FindAsync(Expression<Func<SandMySqlEntity, bool>> predicate, bool includeCustomer = true);
    Task<Result<SandMySqlEntity>> GetByIdAsync(int id, bool includeCustomer = true);
}
