using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.MySql;

namespace LeadPipe.Infrastructure.Data.Source;

public sealed class SubMySqlDataSource(
    ISandMySqlRepository repo
    ) : IDataSourceAsync<SandMySqlEntity>
{
    private readonly ISandMySqlRepository _repo = repo;
    public async Task<Result<List<SandMySqlEntity>>> LoadAsync()
    {
        DateTime twentyTwelve = new(new DateOnly(2012, 1, 1), new TimeOnly(0), DateTimeKind.Utc);
        Result<List<SandMySqlEntity>> found = await _repo.FindAsync(s => s.dateAdded >= twentyTwelve, true);
        return found;
    }

    public async Task<Result<List<SandMySqlEntity>>> RefreshAsync()
    {
        return await LoadAsync();
    }
}