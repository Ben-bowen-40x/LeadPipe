using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Infrastructure.Service.Report;

[SourceKey(Source.Libacion)]
internal sealed class LibacionReportService(
    [FromKeyedServices(Source.Libacion)] ILoadData<Plumbing> load,
    ITransform<Plumbing, ReportFilePlumbing> transform,
    [FromKeyedServices(Source.Libacion)] IReport<ReportFilePlumbing> report
    ) : ReportService<Plumbing, ReportFilePlumbing>(load, transform, report)
{ }
