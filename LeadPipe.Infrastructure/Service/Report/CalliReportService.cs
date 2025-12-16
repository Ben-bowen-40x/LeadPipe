using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Infrastructure.Service.Report;

[SourceKey(Source.Calli)]
internal sealed class CalliReportService(
    [FromKeyedServices(Source.Calli)] ILoadData<Plumbing> load,
    ITransform<Plumbing, ReportFilePlumbing> transform,
    [FromKeyedServices(Source.Calli)] IReport<ReportFilePlumbing> report
    ) : ReportService<Plumbing, ReportFilePlumbing>(load, transform, report)
{ }
