using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

namespace LeadPipe.Infrastructure.Data.Source;

public class MySqlDataSource
{
    protected static async Task<Result> SyncStateAsync(ISyncStateRepository sync, DateTimeOffset dateUpdated, SyncKey key)
    {
        SyncStateEntity state = new()
        {
            BusinessId = BusinessId.From(key.Value),
            LastSyncUtc = dateUpdated.UtcDateTime,
            UnixLastSyncUtc = dateUpdated.ToUnixTimeSeconds()
        };

        Result<List<SyncStateEntity>> upsert = await sync.UpsertRangeAsync([state]);

        return upsert;
    }

    protected static async Task<DateTimeOffset> LatestSyncDate(ISyncStateRepository sync, DateTimeOffset allowableDate, SyncKey key)
    {
        Result<SyncStateEntity> state = await sync.GetByKeyAsync(null, key);
        DateTimeOffset syncDate = state.IsFailure
            ? allowableDate
            : DateTimeOffset.FromUnixTimeSeconds(state.Value.UnixLastSyncUtc);

        return syncDate;
    }

}