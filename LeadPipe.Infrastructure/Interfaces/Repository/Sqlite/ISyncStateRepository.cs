using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface ISyncStateRepository
{
    Task<Result<List<SyncStateEntity>>> UpsertRangeAsync(List<SyncStateEntity> entities);
    Task<Result<SyncStateEntity>> GetByKeyAsync(Source? source, SyncKey key);
    Task<Result<SyncStateEntity>> GetByIdAsync(BusinessId id);
}
