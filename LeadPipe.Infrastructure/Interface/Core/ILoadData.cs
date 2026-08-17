using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Core;

public interface ILoadData<T>
{
    Task<Result<List<T>>> LoadAsync(bool withDetails);
}
