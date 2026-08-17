using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Core;

public interface IDataSourceAsync<T>
{
    Task<Result<List<T>>> LoadAsync(bool withDetails);
    Task<Result<List<T>>> RefreshAsync(bool withDetails);
}
