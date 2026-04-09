using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Manager;

public interface ICoreDataUpdateManager
{
    Task<Result> Manage(ForceRunRefresh frr);
    Task<Result> Manage(ForceRunRefresh frr, params SyncKey[] keys);
}

internal class CoreDataUpdateManager : ICoreDataUpdateManager
{
    private readonly Dictionary<SyncKey, Func<ForceRunRefresh, Task<Result>>> _handlers;

    public CoreDataUpdateManager(
        ISyncGate syncGate,
        IUpdateFactory updateFactory
    )
    {
        var caliper = updateFactory.GetService<Caliper>();
        var custard = updateFactory.GetService<Custard>();
        var sandwich = updateFactory.GetService<Sandwich>();
        var cornFormula = updateFactory.GetService<CornFormula>();

        _handlers = new()
        {
            { SyncKey.Caliper, (frr) => RunIfDue(frr, false, caliper, syncGate) },
            { SyncKey.Custard, (frr) => RunIfDue(frr, true, custard, syncGate) },
            { SyncKey.Sandwich, (frr) => RunIfDue(frr, true, sandwich, syncGate) },
            { SyncKey.CornFormula, (frr) => RunIfDue(frr, false, cornFormula, syncGate) },
        };
    }

    private static string InvalidKey(SyncKey key) => $"Invalid key: {key}";

    public Task<Result> Manage(ForceRunRefresh frr) => Manage(frr, [.. _handlers.Keys]);

    public async Task<Result> Manage(ForceRunRefresh frr, params SyncKey[] keys)
    {
        foreach (var key in keys)
        {
            if (!_handlers.TryGetValue(key, out var handler))
                return Result.Failure(InvalidKey(key));

            var result = await handler(frr);

            if (result.IsFailure)
                return result;
        }
        return Result.Success();
    }

    private static async Task<Result> RunIfDue<T>(ForceRunRefresh frr, bool withDetails, IUpdateService<T> service, ISyncGate syncGate)
    {
        bool shouldRun = await syncGate.ShouldRunAsync(null, service.SyncKey);
        if (!shouldRun && !frr.ForceRun)
            return Result.Success();

        Result result = await UpdatedAndSaved(frr.Refresh, withDetails, service);

        if (result.IsSuccess)
            await syncGate.MarkSuccessAsync(null, service.SyncKey);
        else
            await syncGate.MarkFailureAsync(null, service.SyncKey);

        return result;
    }

    private static async Task<Result> UpdatedAndSaved<T>(bool refresh, bool withDetails, IUpdateService<T> updateService)
    {
        Result<List<T>> updateData = refresh
            ? await updateService.UpdateDataAsync(withDetails)
            : await updateService.GetDataAsync(withDetails);
        Result savedData = updateData.IsSuccess
            ? await updateService.SaveDataAsync(updateData.Value)
            : updateData;
        return savedData;
    }

}
