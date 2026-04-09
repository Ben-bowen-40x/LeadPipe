using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Interfaces.Api;

namespace LeadPipe.Infrastructure.Api;

internal sealed class OAuthHeaderHandler(
    IOAuthTokenProvider provider
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken ct)
    {
        Result<string> token = await provider.GetValidAccessTokenAsync(ct);
        if (token.IsFailure)
            throw new InvalidOperationException($"Token failure: {token.Error}");

        request.Headers.Authorization = new("Bearer", token.Value);

        return await base.SendAsync(request, ct);
    }

}
