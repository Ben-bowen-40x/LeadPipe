using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Core;

internal interface IReport<T>
{
    Task<Result> ReportData(List<T> d);
}
