using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interfaces.Core;

public interface IDataSourceAsync<T>
{
    Task<Result<List<T>>> LoadAsync(bool withDetails);
    Task<Result<List<T>>> RefreshAsync(bool withDetails);
}
