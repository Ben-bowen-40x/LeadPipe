using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

namespace LeadPipe.Infrastructure.Data.Source;

public class MySqlDataSource(ISyncStateRepository sync)
{
    private readonly ISyncStateRepository _sync = sync;
    protected async Task<Result> SyncStateAsync(DateTimeOffset dateUpdated, SyncKey key)
    {
        SyncStateEntity state = new()
        {
            BusinessId = BusinessId.From(key.Value),
            LastSyncUtc = dateUpdated.UtcDateTime,
            UnixLastSyncUtc = dateUpdated.ToUnixTimeSeconds()
        };

        Result<List<SyncStateEntity>> upsert = await _sync.UpsertRangeAsync([state]);

        return upsert;
    }

    protected async Task<DateTimeOffset> LatestSyncDate(DateTimeOffset earliestAllowableDate, SyncKey key)
    {
        Result<SyncStateEntity> state = await _sync.GetByKeyAsync(null, key);
        DateTimeOffset syncDate = state.IsFailure
            ? earliestAllowableDate
            : DateTimeOffset.FromUnixTimeSeconds(state.Value.UnixLastSyncUtc);

        return syncDate;
    }

}