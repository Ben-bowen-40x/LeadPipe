using LeadPipe.Infrastructure.Dto;

namespace LeadPipe.Infrastructure.Interface.Translate;

public interface IEntityToYellerReportFactory
{
    IEntityToReport<TEntity, ReportYeller> GetService<TEntity>();
}