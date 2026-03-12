using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface ISyncStampRepository
{
    Task<Result<SyncStampEntity>> GetByKeyAsync(Source? source, SyncKey key);
    Task<Result<SyncStampEntity>> UpsertAsync(SyncStampEntity entity);
}