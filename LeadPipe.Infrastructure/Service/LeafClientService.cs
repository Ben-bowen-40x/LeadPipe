using CSharpFunctionalExtensions;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using LeadPipe.Application.Service;
using LeadPipe.Domain.FunctionalObjects;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Repository;
using LeadPipe.Infrastructure.Settings;
using LeadPipe.Infrastructure.Translate;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Json;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LeadPipe.Infrastructure.Service;

internal class LeafClientService(
    ILeafSettings settings,
    IDtoToVo dtoTranslate,
    IJsonRwService json,
    IHttpClientFactory factory,
    IPlumbingRepository repo,
    ILogger<LeafClientService> logger) : ILeafClientService
{
    #region Private
    private readonly ILogger<LeafClientService> _logger = logger;
    private readonly ILeafSettings _settings = settings;
    private readonly IDtoToVo _dto = dtoTranslate;
    private readonly IJsonRwService _json = json;
    private readonly IHttpClientFactory _factory = factory;
    private readonly IPlumbingRepository _repo = repo;
    private HttpClient? _client;
    private HttpClient Client => _client ??= _factory.CreateClient(_settings.LeafName!);
    private List<LeafDto>? _leafs;
    private Result SetLeafs(List<LeafDto> leafs)
    {
        if (leafs.Count == 0)
            return Result.Failure("Input list is empty");
        _leafs = leafs;
        return Result.Success();
    }
    private Result<List<LeafDto>> GetLeafs() => _leafs is null || _leafs.Count == 0 ? Result.Failure<List<LeafDto>>("Leaf dto list is null or empty") : _leafs;
    private Uri LeafThreadUrl(int offset = 0, int limit = 1000) => new($"{_settings.LeafThreadsEndpoint}?limit={limit}&offset={offset}");
    private Uri LeafMessagesUrl(string thread, int limit = 10, string type = "sms") => new($"{_settings.LeafThreadsEndpoint}/{thread}{_settings.LeafMessagesEndpoint}?limit={limit}&type={type}&offset=0");
    private static async void Wait(int sleepInterval = 500) => await Task.Delay(sleepInterval);
    private static async Task<Result<T>> GetSingleAsync<T>(Uri url, HttpClient client)
    {
        // Attempt to make the call
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            Wait();
            if (response.IsSuccessStatusCode)
            {
                T? value = await response.Content.ReadFromJsonAsync<T>();
                if (value is not null)
                {
                    return value!;
                }

                string str = await response.Content.ReadAsStringAsync();
                string error = string.IsNullOrWhiteSpace(str) || str.Length == 0
                    ? "Parsing failure. The process of reading the results from Json failed. The results somehow became null."
                    : str;

                return Result.Failure<T>(error);
            }
            return Result.Failure<T>(response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>(ex.Message);
        }
    }
    #endregion

    #region Public
    public async Task<Result<List<Plumbing>>> RefreshAsync(int errorLimit = 5)
    {
        // Retrieve leaf sourced items from database to update database
        Result<List<PlumbingEntity>> plumbingEntities = await _repo.GetAllAsync();

        List<PlumbingEntity>? leafPlumbing = plumbingEntities.IsSuccess
            ? [.. plumbingEntities.Value.Where(v => v.Source == Source.Leaf)]
            : null;
        if (leafPlumbing is null)
            return await GetAllAsync(errorLimit);
        return await GetAllAsync(offset: leafPlumbing.Count - 1, errorLimit);
    }
    public async Task<Result<List<Plumbing>>> GetAllAsync(int offset = 0, int errorLimit = 5)
    {
        const int limit = 1000;
        int errorCount = 0;
        List<Plumbing> master = [];
        List<LeafDto> raw = [];

        bool resume = true;
        bool failure = false;
        while (resume)
        {
            if (errorCount == errorLimit)
            {
                failure = true;
                break;
            }

            try
            {
                // Call the api
                Uri newurl = LeafThreadUrl(offset, limit);
                Result<List<LeafDto>> result = await GetSingleAsync<List<LeafDto>>(newurl, Client);

                // Unwrap result
                if (result.IsSuccess)
                {
                    // Add dtos to raw list
                    List<LeafDto> value = result.Value;
                    value.ForEach(raw.Add);

                    resume = raw.Count == limit;
                }
                else
                {
                    errorCount++;
                    _logger.LogWarning("API call failed at offset {Offset}. Error count: {ErrorCount}. Error Limit: {ErrorLimit}. Error: {Error}", offset, errorCount, errorLimit, result.Error);
                }
                offset += limit;
            }
            catch (Exception e)
            {
                errorCount++;
                _logger.LogError(e, "Exception occurred while calling API. Offset {Offset}. Error Count: {ErrorCount}. Error Limit: {ErrorLimit}", offset, errorCount, errorLimit);
            }
        }

        // Refresh messages
        Result<List<Message>>[] msgs = GetMessages(raw);

        // Reset Leafs using GroupJoin
        var updatedLeafs = raw
            .GroupJoin(
                msgs.Where(r => r.IsSuccess && r.Value is not null)
                    .SelectMany(r => r.Value),
                leaf => leaf.uuid,           // key from raw leaf
                msg => msg.thread,           // key from message
                (leaf, relatedMsgs) =>
                {
                    leaf.messages = [.. relatedMsgs];
                    return leaf;
                }
            ).ToList();

        // Translate from dto to vo
        List<Plumbing> translation = [.. updatedLeafs.Select(_dto.Translate)];
        translation.ForEach(master.Add);

        if (master.Count == 0)
        {
            string msg = "Failed to retrieve any data from the api";
            _logger.LogError("{Message}", msg);
            return Result.Failure<List<Plumbing>>(msg);
        }

        // Save raw, assuming there were raw values
        if (raw.Count > 0)
        {
            SetLeafs(raw);
            FileInfo file = new(FolderFinder.GetLocalFile(nameof(Infrastructure), ".info", "RawLeaf.json"));
            Result saved = _json.WriteToFile(file, raw);
            if (saved.IsFailure)
                _logger.LogError("Failed to write raw dtos to file {FileName} due to {Error}", file.FullName, saved.Error);
        }

        if (failure)
        {
            _logger.LogError("Reached error limit {ErrorLimit}", errorLimit);
            return Result.Failure<List<Plumbing>>($"Reached error limit. Error limit: {errorLimit}");
        }

        return master;
    }
    #endregion

    #region Internal
    internal Result<List<Message>>[] GetMessages(List<LeafDto> leafs)
    {
        List<Task<Result<List<Message>>>> tasks = new(leafs.Count);
        foreach (LeafDto leaf in leafs)
        {
            // Retrieve the thread id
            if (leaf.uuid is null)
                continue;
            string threadid = leaf.uuid;
            Uri uri = LeafMessagesUrl(threadid);

            // Retrieve new list
            Task<Result<List<Message>>> messagesResultTask = GetSingleAsync<List<Message>>(uri, Client);
            tasks.Add(messagesResultTask);

            Thread.Sleep(1000 / (5 - 2));
        }

        Task<Result<List<Message>>[]> completedTask = Task.WhenAll(tasks);
        Result<List<Message>>[] result = ConvertTasks(completedTask);

        return result;
    }

    internal Result<List<T>>[] ConvertTasks<T>(Task<Result<List<T>>[]> completedTask)
    {
        if (completedTask.IsCompletedSuccessfully)
        {
            Result<List<T>>[] taskResult = completedTask.Result;
            IEnumerable<Result<List<T>>> result = taskResult.Select(r =>
            {
                // Unwrapt the value
                if (r.IsFailure) return Result.Failure<List<T>>(r.Error);

                List<T> value = r.Value;
                return Result.Success(value);
            });
            return [.. result];
        }
        else
        {
            string e = completedTask.Exception is not null
                ? completedTask.Exception.Message
                : "Unknown exception";
            _logger.LogError("Error: {Error}", e);
            return [Result.Failure<List<T>>(e)];
        }
    }
    #endregion
}