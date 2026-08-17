using LeadPipe.Infrastructure.Interface.Core;

namespace LeadPipe.Infrastructure.Interface.Repository;

public interface IRepositoryFactory
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
}