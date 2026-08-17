using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Core;

public interface IDataPersistence<T>
{
    Task<Result> SaveAsync(List<T> t);
}
