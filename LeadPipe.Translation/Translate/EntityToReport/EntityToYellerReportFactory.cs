using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Translate;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Translation.Translate.EntityToReport;

internal sealed class EntityToYellerReportFactory(IServiceProvider provider) : IEntityToYellerReportFactory
{
    private readonly IServiceProvider _provider = provider;
    public IEntityToReport<TEntity, ReportYeller> GetService<TEntity>()
    {
        return _provider.GetRequiredService<IEntityToReport<TEntity, ReportYeller>>();
    }
}