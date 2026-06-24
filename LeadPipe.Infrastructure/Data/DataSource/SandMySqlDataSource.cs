using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository;

namespace LeadPipe.Infrastructure.Data.DataSource;

public sealed class SandMySqlDataSource(
    ISandMySqlRepository repo
) : IDataSourceAsync<SandMySqlEntity>
{
    private readonly ISandMySqlRepository _repo = repo;
    public async Task<Result<List<SandMySqlEntity>>> LoadAsync(bool withDetails)
    {
        DateTime twentyTwelve = new(new DateOnly(2012, 1, 1), new TimeOnly(0), DateTimeKind.Utc);
        Result<List<SandMySqlEntity>> found = await _repo.FindAsync(s => s.dateAdded >= twentyTwelve, withDetails);
        return found;
    }

    public async Task<Result<List<SandMySqlEntity>>> RefreshAsync(bool withDetails) => await LoadAsync(withDetails);
}

public sealed class SandMySqlDataSourceBased(
    ISandMySqlRepository repo,
    ISyncStateRepository sync,
    IClock clock
) : SyncedDataSourceBase<SandMySqlEntity>(sync, clock)
{
    private readonly ISandMySqlRepository _repo = repo;
    protected override SyncKey Key => SyncKey.Sandwich;

    protected override DateTimeOffset GetLatest(Result<List<SandMySqlEntity>> entities)
    => entities.IsSuccess && entities.Value.Count > 0
            ? new(entities.Value.Max(v => v.dateAdded) ?? _clock.UtcNow.UtcDateTime.AddDays(-30), TimeSpan.Zero)
            : _clock.UtcNow.AddDays(-30);

    protected override async Task<Result<List<SandMySqlEntity>>> Load(bool withDetails)
    {
        DateTime twentyTwelve = new(2012, 1, 1, 0, 0, 0);
        Result<List<SandMySqlEntity>> found = await _repo.FindAsync(s => s.dateAdded >= twentyTwelve, withDetails);
        return found;
    }

    protected override async Task<Result<List<SandMySqlEntity>>> Refresh(DateTimeOffset latest, bool withDetails)
    {
        DateTime syncDate = latest.UtcDateTime.AddDays(-7);
        Result<List<SandMySqlEntity>> found = await _repo.FindAsync(s => s.dateAdded >= syncDate, withDetails);
        return found;
    }
}