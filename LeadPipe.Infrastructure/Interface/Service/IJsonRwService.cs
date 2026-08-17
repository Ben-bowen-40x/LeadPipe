using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Service;

public interface IJsonRwService
{
    Result<List<T>> ReadFile<T>(FileInfo path);
    Result WriteToFile<T>(FileInfo path, List<T> items);
    Task<Result> WriteToFileAsync<T>(FileInfo path, List<T> items);
}
