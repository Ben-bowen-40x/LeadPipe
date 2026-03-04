using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Service;

namespace LeadPipe.Infrastructure.Data.Source;

public class CatManDataSource(ICatManService cat, ISyncStateRepository state) : IDataSourceAsync<CatManDto>
{
    private readonly ICatManService _cat = cat;
    private readonly ISyncStateRepository _state = state;
    private readonly DateTime Today = DateTime.UtcNow;
    private readonly DateTimeOffset Now = DateTimeOffset.UtcNow;
    public async Task<Result<List<CatManDto>>> LoadAsync(bool _ = default)
    {
        DateTime twentyTwelve = new(2025, 1, 1);
        Result<List<CatManDto>> get = await _cat.GetAllAsync(twentyTwelve, Today);

        var syncDate = GetDate(get);
        await SyncStateAsync(syncDate);
        return get;
    }

    public async Task<Result<List<CatManDto>>> RefreshAsync(bool _ = default)
    {
        // Get most recent refresh date
        Result<SyncStateEntity> state = await _state.GetByKeyAsync(null, SyncKey.Catman);
        if (state.IsFailure)
            return await LoadAsync();

        DateTime lastSync = DateTimeOffset.FromUnixTimeSeconds(state.Value.UnixLastSyncUtc).UtcDateTime;
        Result<List<CatManDto>> result = await _cat.GetAllAsync(lastSync, Today);

        DateTimeOffset syncDate = GetDate(result);
        await SyncStateAsync(syncDate);
        return result;
    }
    internal DateTimeOffset GetDate(Result<List<CatManDto>> get)
    {
        return get.IsSuccess
            ? DateTimeOffset.FromUnixTimeSeconds(get.Value.Min(v => v.unix_time) ?? Now.ToUnixTimeSeconds())
            : Now;
    }
    private async Task<Result> SyncStateAsync(DateTimeOffset date)
    {
        SyncStateEntity catmanstate = new()
        {
            BusinessId = BusinessId.From(SyncKey.Catman.Value),
            LastSyncUtc = date.UtcDateTime,
            UnixLastSyncUtc = date.ToUnixTimeSeconds()
        };
        var upsert = await _state.UpsertRangeAsync([catmanstate]);
        return upsert;
    }
}
