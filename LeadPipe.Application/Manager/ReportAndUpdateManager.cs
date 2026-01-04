using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Manager;

public interface IReportAndUpdateManager
{
    Task<Result> Manage(Source source, bool refresh, UpdateReportManagement manage);
    Task<Result> Manage(bool refresh, UpdateReportManagement manage);
}
internal class ReportAndUpdateManager(
    IUpdateSourceFactory update,
    IReportSourceFactory report,
    IUpdateService<Call> updateCall,
    IUpdateService<Sandwich> updateSandwich,
    IPlumbingAssociationService plumb
    ) : IReportAndUpdateManager
{
    private readonly IUpdateSourceFactory _update = update;
    private readonly IReportSourceFactory _report = report;
    private readonly IUpdateService<Call> _updateCall = updateCall;
    private readonly IUpdateService<Sandwich> _updateSandwich = updateSandwich;
    private readonly IPlumbingAssociationService _plumb = plumb;

    public async Task<Result> Manage(Source source, bool refresh, UpdateReportManagement manage)
    {
        if (manage.Update)
        {
            // Update
            IUpdateService<Plumbing> updateService = _update.GetService(source);
            Result savedData = await UpdatedAndSaved(refresh, updateService);

            // Call data
            Result savedCall = await UpdatedAndSaved(refresh, _updateCall);

            // Sandwich data
            Result savedSandwich = await UpdatedAndSaved(refresh, _updateSandwich);

            // Associate
            Result associated = await _plumb.SaveLinksAsync();

            // Combine
            Result updateResult = Result.Combine(" | ", savedData, savedCall, savedSandwich, associated);
            if (updateResult.IsFailure)
                return updateResult;
        }
        if (manage.Report)
        {
            // Report Plumbing
            IReportService<Plumbing> reportService = _report.GetService(source);
            Result<List<Plumbing>> data = await reportService.GetDataAsync();
            Result reported = data.IsSuccess
                ? await reportService.ReportAsync(data.Value)
                : data;

            return reported;
        }
        return Result.Failure($"Management is malformed. Update and Report cannot both be false. Update: {manage.Update}. Report: {manage.Report}.");
    }

    public async Task<Result> Manage(bool refresh, UpdateReportManagement manage)
    {
        if (!manage.Report && !manage.Update)
            return Result.Failure($"Malformed management. 'Update' and 'Report' cannot both be false. Update: {manage.Update}. Report: {manage.Report}.");

        if (manage.Update)
        {
            // Call data
            Result savedCall = await UpdatedAndSaved(refresh, _updateCall);

            // Sandwich data
            Result savedSandwich = await UpdatedAndSaved(refresh, _updateSandwich);

            // Associate
            Result associated = await _plumb.SaveLinksAsync();

            Result updateResult = Result.Combine(" | ", savedCall, savedSandwich, associated);
            if (updateResult.IsFailure)
                return updateResult;
        }

        // Update and Report
        List<Result> result = [];
        Source[] values = Enum.GetValues<Source>();
        foreach (Source source in values)
        {
            if (manage.Update)
            {
                IUpdateService<Plumbing> update = _update.GetService(source);

                // Save Data
                Result saved = await UpdatedAndSaved(refresh, update);

                if (saved.IsFailure)
                {
                    result.Add(saved);
                    continue; // If we're updating and reporting, we can't do the report if the save failed
                }
            }

            if (manage.Report)
            {
                // Report Data
                IReportService<Plumbing> report = _report.GetService(source);
                Result<List<Plumbing>> reportData = await report.GetDataAsync();
                Result reported = reportData.IsSuccess
                    ? await report.ReportAsync(reportData.Value)
                    : reportData;
                result.Add(reported);
            }

        }

        // Return result
        return Result.Combine(" | ", [.. result]);
    }
    private static async Task<Result> UpdatedAndSaved<T>(bool refresh, IUpdateService<T> updateService)
    {
        Result<List<T>> updateData = refresh
            ? await updateService.UpdateDataAsync()
            : await updateService.GetDataAsync();
        Result savedData = updateData.IsSuccess
            ? await updateService.SaveDataAsync(updateData.Value)
            : updateData;
        return savedData;
    }
}
