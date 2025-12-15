using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;

namespace LeadPipe.Infrastructure.Service;

[SourceKey(Source.Yeller)]
public class YellerClientReporter : IReport<ReportYeller>
{
    public Task<Result> ReportData(List<ReportYeller> d)
    {
        throw new NotImplementedException();
    }
}
