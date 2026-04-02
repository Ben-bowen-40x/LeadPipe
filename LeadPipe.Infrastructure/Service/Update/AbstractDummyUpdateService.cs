using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Attributes;

namespace LeadPipe.Infrastructure.Service.Update;

internal abstract class AbstractDummyUpdateService(SyncKey key): IUpdateService<Plumbing>
{
    public SyncKey SyncKey => key;

    public Task<Result<List<Plumbing>>> GetDataAsync(bool _)
    {
        return Task.FromResult(Result.Success(new List<Plumbing>()));
    }

    public Task<Result> SaveDataAsync(List<Plumbing> data)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result<List<Plumbing>>> UpdateDataAsync(bool _)
    {
        return Task.FromResult(Result.Success<List<Plumbing>>([]));
    }
}
[SourceKey(Source.Test)]
internal sealed class DummyUpdateService() : AbstractDummyUpdateService(SyncKey.Plumbing), IUpdateService<Plumbing> { }
[SourceKey(Source.Test2)]
internal sealed class DummyUpdateService2() : AbstractDummyUpdateService(SyncKey.Plumbing), IUpdateService<Plumbing> { }