using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Service;
using LeadPipe.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace LeadPipe.Infrastructure.Service;

internal class CatManClientService(
    ICatManSettings settings, 
    IHttpClientFactory factory,
    ILogger<CatManClientService> logger
    ) : ICatManService
{
    private readonly ICatManSettings _settings = settings;
    private readonly HttpClient _client = factory.CreateClient(settings.CatManClientName!);
    private readonly ILogger<CatManClientService> _logger = logger;

    private const int MaxRequestsPerSecond = 8;
    private static readonly TimeSpan RateLimitDelay = TimeSpan.FromMilliseconds(1000d / MaxRequestsPerSecond);

    private string BuildCallEndpoint(DateTime start, DateTime end)
    {
        var formattedStart = start.ToString(_settings.CatManDateFormat!);
        var formattedEnd = end.ToString(_settings.CatManDateFormat!);

        var endpoint = $"accounts/{_settings.CatAccountId}/calls.json" +
                  $"?start_date={formattedStart}&end_date={formattedEnd}" +
                  "&direction[]=form";

        return endpoint;
    }

    private async Task<Result<CatManRootDto>> GetCallAsync(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                return Result.Failure<CatManRootDto>(response.ReasonPhrase ?? $"{nameof(CatManRootDto)} request failed");

            var value = await response.Content.ReadFromJsonAsync<CatManRootDto>();

            if (value is not null)
                return Result.Success(value);
            var raw = await response.Content.ReadAsStringAsync();

            return Result.Failure<CatManRootDto>(
                string.IsNullOrWhiteSpace(raw)
                ? $"{nameof(CatManRootDto)} deserialization returned null"
                : raw);
        }
        catch (Exception ex) { return Result.Failure<CatManRootDto>(ex.ToString()); }
    }

    public async Task<Result<List<CatManDto>>> GetAllAsync(DateTime start, DateTime end)
    {
        List<CatManDto> results = [];

        string nextEndpoint = BuildCallEndpoint(start, end);
        string? afterValueOfPreviousPage = null;

        while (nextEndpoint is not null)
        {
            Result<CatManRootDto> result = await GetCallAsync(nextEndpoint);

            if (result.IsFailure)
                return Result.Failure<List<CatManDto>>(result.Error);

            var calls = result.Value.calls ?? [];
            if (result.Value.calls is null || result.Value.calls.Length == 0)
                _logger.LogWarning("The following endpoint returned null or empty values: {Endpoint}", nextEndpoint);

            results.AddRange(calls);

            await Task.Delay(RateLimitDelay);

            if (string.IsNullOrWhiteSpace(result.Value.next_page) || result.Value.after == afterValueOfPreviousPage)
                break;

            afterValueOfPreviousPage = result.Value.after;
            nextEndpoint = new(result.Value.next_page);
        }
        return Result.Success(results);
    }
}
