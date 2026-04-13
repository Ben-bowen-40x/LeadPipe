using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Api;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LeadPipe.Infrastructure.Api;

internal abstract class OAuthTokenProvider<T>(
    ITokenCacheService cache,
    IOAuthTokenRepository tokenRepository,
    IHttpClientFactory httpClientFactory,
    IClock clock,
    ITranslate<TokenDto, OAuthTokenEntity> translate,
    ILogger<T> logger,
    string providerName) : IOAuthTokenProvider
{
    readonly ITokenCacheService _cache = cache;
    readonly IOAuthTokenRepository _tokenPersistence = tokenRepository;
    readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    readonly IClock _clock = clock;
    readonly ITranslate<TokenDto, OAuthTokenEntity> _translate = translate;
    readonly ILogger<T> _logger = logger;
    readonly string _providerName = providerName;
    protected abstract Task<Result<FormUrlEncodedContent>> Content(CancellationToken ct);
    protected abstract Uri AuthorizationUri { get; }
    protected abstract string OAuthClientName { get; }
    protected virtual int ErrorLimit { get; } = 5;
    protected virtual long BufferSeconds { get; } = 60;

    public async Task<Result<AccessToken>> ForceRefreshAsync(CancellationToken ct)
    {
        HttpClient client = _httpClientFactory.CreateClient(OAuthClientName);

        // Post and check response
        for (var attempt = 0; attempt < ErrorLimit; attempt++)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                
                var contentResult = await Content(ct);
                if (contentResult.IsFailure)
                    return Result.Failure<AccessToken>(contentResult.Error);
                using HttpResponseMessage response = await client.PostAsync(AuthorizationUri, contentResult.Value, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Provider={Provider}. Status Code={StatusCode}. Reason Phrase={ReasonPhrase}. Total Errors={Errors}",
                        _providerName,
                        response.StatusCode,
                        response.ReasonPhrase,
                        attempt + 1);
                    if (attempt == ErrorLimit - 1)
                        return Result.Failure<AccessToken>(response.ReasonPhrase);
                    continue;
                }

                // Convert the response
                // Cancellation.None is necessary because if we've reached this point, this operation is CRITICAL; without it, we have to manually reset the token, 
                // a process that is manual and error-prone
                string responseString = await response.Content.ReadAsStringAsync(CancellationToken.None);
                TokenDto? tokenDto = JsonSerializer.Deserialize<TokenDto>(responseString);
                if (tokenDto is null)
                {
                    _logger.LogError("Deserialization Error. Provider={Provider}. Total Errors={Errors}. Response String={ResponseString}",
                        _providerName,
                        attempt + 1,
                        responseString
                        );
                    if (attempt == ErrorLimit - 1)
                        return Result.Failure<AccessToken>($"{nameof(ForceRefreshAsync)}: Failed to deserialize token.");
                    continue;
                }

                // Translate the token
                tokenDto.Provider = _providerName;
                OAuthTokenEntity e = _translate.Translate(tokenDto);
                e.Provider = _providerName;

                // Persist the token
                // Cancellation.None is necessary because if we've reached this point, this operation is CRITICAL; without it, we have to manually reset the token, 
                // a process that is manual and error-prone
                Result<OAuthTokenEntity> persisted = await _tokenPersistence.UpsertAsync(e, CancellationToken.None);
                if (persisted.IsFailure)
                {
                    _logger.LogError("Provider failed to persist the token. Provider={Provider}. Token={Token}. Total Errors={Errors}. Error Message={ErrorMessage}",
                        _providerName,
                        tokenDto,
                        attempt + 1,
                        persisted.Error);
                    if (attempt == ErrorLimit - 1)
                        return Result.Failure<AccessToken>(persisted.Error);
                    continue;
                }

                return FromEntity(persisted.Value);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Execution error. Provider={Provider}", _providerName);
                if (attempt == ErrorLimit - 1)
                    return Result.Failure<AccessToken>(ex.Message);
                continue;
            }
        }

        _logger.LogError("Provider failed to provide token after several attempts. Provider={Provider}. Attempts={Attempts}",
            _providerName,
            ErrorLimit);
        return Result.Failure<AccessToken>($"{_providerName} failed to fetch token");
    }

    /// <summary>
    /// Checks persistence for existing token. If the token is not expired, it returns the existing token. If the token is expired or about to expire, calls api for a new token, persists the token, and returns the new token.
    /// </summary>
    /// <param name="_providerName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<string>> GetValidAccessTokenAsync(CancellationToken ct)
    {
        var inMemory = await _cache.GetOrAddAsync(
            key: $"oauth::{_providerName}",
            factory: GetValidAccessTokenInternal,
            bufferSeconds: BufferSeconds,
            ct: ct);
        return inMemory;
    }

    private async Task<Result<AccessToken>> GetValidAccessTokenInternal(CancellationToken ct)
    {
        // Retrieve existing token
        var entity = await _tokenPersistence.GetByProviderAsync(_providerName, ct);
        if (entity.IsFailure)
            return await ForceRefreshAsync(ct);

        // If token is expired, refresh it
        var now = _clock.UtcNow.ToUnixTimeSeconds();
        if (entity.Value.UnixExpiresAtUtc <= now + BufferSeconds)
            return await ForceRefreshAsync(ct);

        return FromEntity(entity.Value);
    }

    protected static AccessToken FromEntity(OAuthTokenEntity e) => new(e.AccessToken, e.UnixExpiresAtUtc);
}
