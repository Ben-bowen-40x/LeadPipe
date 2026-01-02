using CSharpFunctionalExtensions;
using LeadPipe.Application.UpdateReportPipeline;
using LeadPipe.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace LeadPipe.Application.Manager;

public interface IUpdateAndReportAllManager
{
    Task<Result> Manage(UpdateReportManagement manage);
    Task<Result> Manage(Source source, UpdateReportManagement manage);
}

internal sealed class UpdateAndReportAllManager(
    IUpdateCalliManager calliUpdate,
    IUpdateCallsManager callsUpdate,
    IUpdateLabManager labUpdate,
    IUpdateLeafManager leafUpdate,
    IUpdateLeasedManager leasedUpdate,
    IUpdateLibacionManager libacionUpdate,
    IUpdatePanManager panUpdate,
    IUpdateSandwichManager sandwichUpdate,
    IUpdateYellerManager yellerUpdate,

    IPlumbingAssociationManager associate,

    IReportCalliManager calliReport,
    IReportLabManager labReport,
    IReportLeafManager leafReport,
    IReportLeasedManager leasedReport,
    IReportLibacionManager libacionReport,
    IReportPanManager panReport,
    IReportYellerManager yellerReport
    ) : IUpdateAndReportAllManager
{

    #region Fields
    private readonly IUpdateCalliManager _calliUpdate = calliUpdate;
    private readonly IUpdateCallsManager _callsUpdate = callsUpdate;
    private readonly IUpdateLabManager _labUpdate = labUpdate;
    private readonly IUpdateLeafManager _leafUpdate = leafUpdate;
    private readonly IUpdateLeasedManager _leasedUpdate = leasedUpdate;
    private readonly IUpdateLibacionManager _libacionUpdate = libacionUpdate;
    private readonly IUpdatePanManager _panUpdate = panUpdate;
    private readonly IUpdateSandwichManager _sandwichUpdate = sandwichUpdate;
    private readonly IUpdateYellerManager _yellerUpdate = yellerUpdate;

    private readonly IPlumbingAssociationManager _associate = associate;

    private readonly IReportCalliManager _calliReport = calliReport;
    private readonly IReportLabManager _labReport = labReport;
    private readonly IReportLeafManager _leafReport = leafReport;
    private readonly IReportLeasedManager _leasedReport = leasedReport;
    private readonly IReportLibacionManager _libacionReport = libacionReport;
    private readonly IReportPanManager _panReport = panReport;
    private readonly IReportYellerManager _yellerReport = yellerReport;
    private const string ErrorMessagesSeparator = " | ";
    #endregion

    public async Task<Result> Manage(UpdateReportManagement manage)
    {
        Result manager = await Manage(source: Source.Test, manage);
        return manager;
    }
    public async Task<Result> Manage(Source source, UpdateReportManagement manage)
    {
        if (manage.Update && manage.Report)
        {
            var update = await ManageUpdate(source);
            var report = await ManageReport(source);
            return Result.Combine(ErrorMessagesSeparator, update, report);
        }
        else if (manage.Update)
        {
            var update = await ManageUpdate(source);
            return update;
        }
        else if (manage.Report)
        {
            var report = await ManageReport(source);
            return report;
        }
        else return Result.Failure($"{nameof(UpdateReportManagement)} was malformed: {manage}");
    }

    private async Task<Result> ManageUpdate(Source source)
    {
        Result<List<Call>> callsUpdateResult = await _callsUpdate.ManageAsync();
        Result<List<Sandwich>> sandwichUpdateResult = await _sandwichUpdate.ManageAsync();

        Result<List<Plumbing>> sourceUpdateResult = source switch
        {
            Source.Calli => await _calliUpdate.ManageAsync(),
            Source.Lab => await _labUpdate.ManageAsync(),
            Source.Leaf => await _leafUpdate.ManageAsync(),
            Source.Leased => await _leasedUpdate.ManageAsync(),
            Source.Libacion => await _libacionUpdate.ManageAsync(),
            Source.Pan => await _panUpdate.ManageAsync(),
            Source.Yeller => await _yellerUpdate.ManageAsync(),
            _ => await UpdateAllAsync()
        };

        Result associate = await _associate.ManageAsync();

        Result combined = Result.Combine(ErrorMessagesSeparator, callsUpdateResult, sandwichUpdateResult, sourceUpdateResult, associate);
        return combined;

        async Task<Result<List<Plumbing>>> UpdateAllAsync()
        {
            Result<List<Plumbing>> calliUpdate = await _calliUpdate.ManageAsync();
            Result<List<Plumbing>> labUpdate = await _labUpdate.ManageAsync();
            Result<List<Plumbing>> leafUpdate = await _leafUpdate.ManageAsync();
            Result<List<Plumbing>> leasedUpdate = await _leasedUpdate.ManageAsync();
            Result<List<Plumbing>> libacionUpdate = await _libacionUpdate.ManageAsync();
            Result<List<Plumbing>> panUpdate = await _panUpdate.ManageAsync();
            Result<List<Plumbing>> yellerUpdate = await _yellerUpdate.ManageAsync();
            Result combine = Result.Combine(ErrorMessagesSeparator,
                calliUpdate,
                labUpdate,
                leafUpdate,
                leasedUpdate,
                libacionUpdate,
                panUpdate,
                yellerUpdate);

            if (combine.IsFailure)
                return Result.Failure<List<Plumbing>>(combine.Error);
            return Result.Success<List<Plumbing>>([
                .. calliUpdate.Value,
                .. labUpdate.Value,
                .. leafUpdate.Value,
                .. leasedUpdate.Value,
                .. libacionUpdate.Value,
                .. panUpdate.Value,
                .. yellerUpdate.Value,
                ]);
        }
    }
    private async Task<Result> ManageReport(Source source)
    {
        Result<List<Plumbing>> reportResult = source switch
        {
            Source.Calli => await _calliReport.ManageAsync(),
            Source.Lab => await _labReport.ManageAsync(),
            Source.Leaf => await _leafReport.ManageAsync(),
            Source.Leased => await _leasedReport.ManageAsync(),
            Source.Libacion => await _libacionReport.ManageAsync(),
            Source.Pan => await _panReport.ManageAsync(),
            Source.Yeller => await _yellerReport.ManageAsync(),
            _ => await ReportAllAsync()
        };
        return reportResult;

        async Task<Result<List<Plumbing>>> ReportAllAsync()
        {
            Result<List<Plumbing>> calliReport = await _calliReport.ManageAsync();
            Result<List<Plumbing>> labReport = await _labReport.ManageAsync();
            Result<List<Plumbing>> leafReport = await _leafReport.ManageAsync();
            Result<List<Plumbing>> leasedReport = await _leasedReport.ManageAsync();
            Result<List<Plumbing>> libacionReport = await _libacionReport.ManageAsync();
            Result<List<Plumbing>> panReport = await _panReport.ManageAsync();
            Result<List<Plumbing>> yellerReport = await _yellerReport.ManageAsync();
            Result combine = Result.Combine(ErrorMessagesSeparator,
                calliReport,
                labReport,
                leafReport,
                leasedReport,
                libacionReport,
                panReport,
                yellerReport);

            if (combine.IsFailure)
                return Result.Failure<List<Plumbing>>(combine.Error);
            return Result.Success<List<Plumbing>>([
                .. calliReport.Value,
                .. labReport.Value,
                .. leafReport.Value,
                .. leasedReport.Value,
                .. libacionReport.Value,
                .. panReport.Value,
                .. yellerReport.Value,
                ]);
        }

    }

}