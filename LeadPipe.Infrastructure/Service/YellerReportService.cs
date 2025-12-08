using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Service;

namespace LeadPipe.Infrastructure.Service;

[SourceKey(Source.Yeller)]
internal sealed class YellerReportService(
    ILoadData<Plumbing> load,
    ITransform<Plumbing, YellerReport> transform,
    IReport<YellerReport> report
    ) : ReportService<Plumbing, YellerReport>(load, transform, report), IYellerReportService
{ }
