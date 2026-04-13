using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Api;

namespace LeadPipe.Infrastructure.Interfaces.Api;

internal interface ITokenCacheService
{
    Task<Result<string>> GetOrAddAsync(
        string key,
        Func<CancellationToken, Task<Result<AccessToken>>> factory,
        long bufferSeconds,
        CancellationToken ct);
}

