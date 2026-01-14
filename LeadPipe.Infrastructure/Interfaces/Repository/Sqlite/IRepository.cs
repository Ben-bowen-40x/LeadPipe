using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface IRepository<T> where T : class, IEntity
{
    Task<Result<List<T>>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<Result<List<T>>> GetAllAsync();
    Task<Result<List<T>>> GetAllWithDetailsAsync();
    Task<Result<List<T>>> UpsertRangeAsync(List<T> entities);
}
