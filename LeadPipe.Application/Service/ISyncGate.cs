using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Service;

public interface ISyncGate
{
    Task<bool> ShouldRunAsync(Source? source, SyncKey key);
    Task MarkSuccessAsync(Source? source, SyncKey key);
    Task MarkFailureAsync(Source? source, SyncKey key);
}
