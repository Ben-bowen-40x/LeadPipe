using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Service;

internal sealed class CallsUpdateService(
    IDataSourceAsync<CallMySqlEntity> call,
    IEntityToVo<CallMySqlEntity, Call> eToVo,
    IDataPersistence<Call> persistence
    ) : IUpdateService<Call>
{
    private readonly IDataSourceAsync<CallMySqlEntity> _call = call;
    private readonly IEntityToVo<CallMySqlEntity, Call> _eToVo = eToVo;
    private readonly IDataPersistence<Call> _persistence = persistence;
    public async Task<Result<List<Call>>> GetDataAsync()
    {
        // Retrieve all data from source
        Result<List<CallMySqlEntity>> callsResult = await _call.LoadAsync();
        if (callsResult.IsFailure)
            return Result.Failure<List<Call>>(callsResult.Error);

        Result<List<Call>> value = callsResult.IsSuccess
            ? Result.Success(callsResult.Value.Select(_eToVo.Translate).ToList())
            : Result.Failure<List<Call>>(callsResult.Error);
        return value;
    }

    public async Task<Result> SaveDataAsync(List<Call> data)
    {
        Result saved = await _persistence.SaveAsync(data);
        return saved;
    }

    public async Task<Result<List<Call>>> UpdateDataAsync()
    {
        Result<List<CallMySqlEntity>> callsResult = await _call.RefreshAsync();
        if (callsResult.IsFailure)
            return Result.Failure<List<Call>>(callsResult.Error);

        return callsResult.IsSuccess
            ? Result.Success(callsResult.Value.Select(_eToVo.Translate).ToList())
            : Result.Failure<List<Call>>(callsResult.Error);
    }
}

