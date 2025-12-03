using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Data.Source;

public interface IDataSourceAsync<T>
{
    public Task<Result<List<T>>> LoadAsync();
    Task<Result<List<T>>> RefreshAsync();
}
