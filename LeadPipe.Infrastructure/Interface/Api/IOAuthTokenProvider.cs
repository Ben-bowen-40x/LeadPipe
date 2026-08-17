using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Api;

namespace LeadPipe.Infrastructure.Interface.Api;

internal interface IOAuthTokenProvider
{
    Task<Result<AccessToken>> ForceRefreshAsync(CancellationToken ct);
    Task<Result<string>> GetValidAccessTokenAsync(CancellationToken ct);
}

