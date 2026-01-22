using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Service;
using LeadPipe.Infrastructure.Interfaces.Translate;
using LeadPipe.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace LeadPipe.Infrastructure.Service;

internal record YellerFetchResult(List<string> Raw, int Errors, string FinalId);
internal class YellerClientService : IYellerService
{
    #region Ctor and Private Fields
    private readonly IHttpClientFactory _factory;
    private readonly IYellerSettings _settings;
    private readonly HttpClient _client;
    private readonly IDtoToVo<YellerDto, Plumbing> _dtoToVo;
    private readonly ILogger _logger;
    private readonly ISyncStateRepository _sync;
    private readonly SemaphoreSlim _throttle;
    private const int errorLimit = 5;

    public YellerClientService(
        IHttpClientFactory factory,
        IYellerSettings settings,
        IDtoToVo<YellerDto, Plumbing> dtoToVo,
        ILogger<YellerClientService> logger,
        ISyncStateRepository sync
        )
    {
        _factory = factory;
        _settings = settings;
        _client = _factory.CreateClient(_settings.YellerGetterName!);
        _dtoToVo = dtoToVo;
        _logger = logger;
        _throttle = new SemaphoreSlim(_settings.YellerConcurrentMax);
        _sync = sync;
    }
    #endregion

    const int limit = 20;
    public async Task<Result<List<Plumbing>>> GetAllAsync(string id = "")
    {
        if (_settings.YellerBellerId is null)
            return Result.Failure<List<Plumbing>>($"{nameof(_settings.YellerBellerId)} list is null");
        string[] yellerIds = _settings.YellerBellerId;

        if (yellerIds.Length == 0)
            return Result.Failure<List<Plumbing>>($"{nameof(_settings.YellerBellerId)} list is empty");

        List<string> allRaw = [];
        int allErrors = 0;
        string process = "Id retrieval";
        string finalId = "";
        foreach (var yellerId in yellerIds)
        {
            string endpoint = id == ""
                ? $"{_settings.YellerPrelimEndpoint1}{yellerId}{_settings.YellerPrelimEndpoint2}?limit={limit}"
                : $"{_settings.YellerPrelimEndpoint1}{yellerId}{_settings.YellerPrelimEndpoint2}?limit={limit}&{_settings.YellerPrelimId}={id}";
            YellerFetchResult data = await GetData(yellerId, process, endpoint);

            // Aggregate raw and errors
            allRaw.AddRange(data.Raw);
            allErrors += data.Errors;

            if (!string.IsNullOrWhiteSpace(data.FinalId))
                finalId = data.FinalId;
        }

        // Distinct raw
        allRaw = [.. allRaw.Distinct()];

        // finalId is an opaque, API-defined cursor (alphanumeric).
        // We intentionally persist the last cursor returned by the API,
        // not a computed max, because ordering semantics are undocumented.
        SyncStateEntity state = new() { LastProcessedId = finalId, LastSyncUtc = DateTime.UtcNow, UnixLastSyncUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
        Result<SyncStateEntity> synced = await _sync.SaveAsync(state);
        if (synced.IsFailure)
        {
            _logger.LogError(
                "Failed to sync. The next call to this api will have data that overlaps with existing persisted data. Error {Error}",
                synced.Error);
        }

        // Check that raw is not empty
        if (allRaw.Count == 0)
        {
            _logger.LogError(
                "Failed prelim retrieval. Errors: {Errors}. Process: {Process}",
                allErrors, process);

            return Result.Failure<List<Plumbing>>("Failed to retrieve data from API.");
        }

        // Hydrate dtos with data by fetching data in all ids
        Result<List<YellerDto>> dtoResult = await GetDto(allRaw);

        if (!dtoResult.IsSuccess)
            return Result.Failure<List<Plumbing>>(dtoResult.Error);

        List<Plumbing> final = [.. dtoResult.Value.Select(_dtoToVo.Translate)];
        return Result.Success(final);
    }

    private async Task<YellerFetchResult> GetData(string yellerId, string process, string endpoint)
    {
        List<string> raw = [];
        int errors = 0;
        string finalId = "";
        while (true)
        {
            if (errors >= errorLimit)
            {
                _logger.LogWarning(
                    "Reached error limit {ErrorLimit}. Retrieved: {Retrieved}. Process: {Process}",
                    errorLimit, raw.Count, process);
                break;
            }

            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    errors++;
                    _logger.LogError(
                        "Response failure ({Reason}). Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                        response.ReasonPhrase, errors, errorLimit, raw.Count, process);

                    continue;
                }

                YellerHelperDto? value = await response.Content.ReadFromJsonAsync<YellerHelperDto>();

                if (value?.lead_ids == null || value.lead_ids.Length == 0)
                {
                    errors++;
                    _logger.LogWarning(
                        "Null or invalid prelim response. Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                        errors, errorLimit, raw.Count, process);

                    continue;
                }

                raw.AddRange(value.lead_ids);

                // According to our Point of Contact for this Api, 
                // it may be necessary to fetch the first item because
                // ids are ordered descending, not ascending.
                // So, lead_ids[0] is the most chronologically recent, not the most recent
                // The api has a bug that's currently being worked on as of 1/8/2026
                finalId = value.lead_ids[^1]; // may need replacement by finalId = value.lead_ids[0];
                endpoint = $"{_settings.YellerPrelimEndpoint1}{yellerId}{_settings.YellerPrelimEndpoint1}?limit={limit}&{_settings.YellerPrelimId}={finalId}";

                if (!value.has_more)
                    break;
            }
            catch (Exception ex)
            {
                errors++;
                _logger.LogError(
                    ex,
                    "Exception in prelim stage. Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                    errors, errorLimit, raw.Count, process);
            }
        }

        return new(raw, errors, finalId);
    }

    private async Task<Result<List<YellerDto>>> GetDto(List<string> raw, string process = "Value retrieval")
    {
        if (raw.Count == 0)
            return Result.Failure<List<YellerDto>>("No IDs provided");

        List<YellerDto> master = [];
        int errors = 0;

        foreach (string id in raw)
        {
            if (errors >= errorLimit)
            {
                _logger.LogWarning(
                    "Reached error limit {ErrorLimit}. Retrieved: {Retrieved}. Process: {Process}",
                    errorLimit, master.Count, process);
                break;
            }

            await _throttle.WaitAsync();
            try
            {
                string uri = $"{_settings.YellerFinalEndpoint}/{id}";
                HttpResponseMessage response = await _client.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                {
                    errors++;
                    _logger.LogWarning(
                        "Final response failed. Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                        errors, errorLimit, master.Count, process);
                    continue;
                }

                YellerDto? dto = await response.Content.ReadFromJsonAsync<YellerDto>();

                if (dto == null)
                {
                    errors++;
                    _logger.LogWarning(
                        "Null DTO. Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                        errors, errorLimit, master.Count, process);
                    continue;
                }

                master.Add(dto);
            }
            catch (Exception ex)
            {
                errors++;
                _logger.LogError(
                    ex,
                    "Exception in DTO stage. Errors: {Errors}/{Limit}. Retrieved: {Retrieved}. Process: {Process}",
                    errors, errorLimit, master.Count, process);
            }
            finally { _throttle.Release(); }
        }

        if (master.Count == 0)
            return Result.Failure<List<YellerDto>>("Failed to retrieve DTOs");

        return Result.Success(master);
    }

    public async Task<Result<List<Plumbing>>> RefreshAsync()
    {
        return await GetAllAsync();
    }
}
