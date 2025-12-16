using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Infrastructure.Service.Report;

[SourceKey(Source.Yeller)]
internal sealed class YellerReportService(
    [FromKeyedServices(Source.Yeller)] ILoadData<Plumbing> load,
    ITransform<Plumbing, ReportYeller> transform,
    [FromKeyedServices(Source.Yeller)] IReport<ReportYeller> report
    ) : ReportService<Plumbing, ReportYeller>(load, transform, report)
{ }

[SourceKey(Source.Calli)]
internal sealed class CalliReportService(
    [FromKeyedServices(Source.Calli)] ILoadData<Plumbing> load,
    ITransform<Plumbing, ReportFilePlumbing> transform,
    [FromKeyedServices(Source.Calli)] IReport<ReportFilePlumbing> report
    ) : ReportService<Plumbing, ReportFilePlumbing>(load, transform, report)
{ }