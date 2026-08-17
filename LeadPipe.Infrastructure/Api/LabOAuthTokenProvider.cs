using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Api;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;
using LeadPipe.Infrastructure.Interface.Translate;
using LeadPipe.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace LeadPipe.Infrastructure.Api;

internal sealed class LabOAuthTokenProvider(
    ILabSettings settings,
    ITokenCacheService cache,
    IOAuthTokenRepository tokenRepository,
    IHttpClientFactory httpClientFactory,
    IClock clock,
    ITranslate<TokenDto, OAuthTokenEntity> translate,
    ILogger<LabOAuthTokenProvider> logger,
    string providerName
) : OAuthTokenProvider<LabOAuthTokenProvider>(cache, tokenRepository, httpClientFactory, clock, translate, logger, providerName)
{
    readonly ILabSettings _settings = settings;
    protected override Uri AuthorizationUri => new(_settings.LabAuthorizationUrl!);

    protected override string OAuthClientName => _settings.LabOAuthName!;

    protected override Task<Result<FormUrlEncodedContent>> Content(CancellationToken _)
    {
        if(_settings.LabId is null)
            return Task.FromResult(Result.Failure<FormUrlEncodedContent>($"{nameof(_settings.LabId)} cannot be null"));
        if (_settings.LabSecret is null)
            return Task.FromResult(Result.Failure<FormUrlEncodedContent>($"{nameof(_settings.LabSecret)} cannot be null"));

        Dictionary<string, string> rawContent = new()
        {
            { "grant_type", "client_credentials" },
            { "client_id", _settings.LabId},
            { "client_secret", _settings.LabSecret! }
        };
        FormUrlEncodedContent content = new(rawContent);

        return Task.FromResult(Result.Success(content));
    }
}
