using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Service;
using LeadPipe.Infrastructure.Interfaces.Translate;
using LeadPipe.Infrastructure.Settings;

namespace LeadPipe.Infrastructure.Data.Transform;

internal sealed class TransformYellerReport(
    IRepositoryFactory factory,
    IEntityToReport<PlumbingEntity, ReportYeller> plumbToR,
    IEntityToReport<CaliperEntity, ReportYeller> caliperToR,
    IEntityToReport<CornEntity, ReportYeller> cornToR,
    IEntityToReport<CustardPlumbingLink, ReportYeller> cpLinkToR,
    IEntityToReport<CustardCaliperLink, ReportYeller> custCalToR,
    IEntityToReport<CustardCornLink, ReportYeller> custCornToR,
    IYellerSettings settings
    ) : ITransform<Plumbing, ReportYeller>
{

    private readonly IRepository<CustardEntity> _custardRepo = factory.GetRepository<CustardEntity>();
    private readonly IRepository<CaliperEntity> _caliperRepo = factory.GetRepository<CaliperEntity>();
    private readonly IRepository<CornEntity> _cornRepo = factory.GetRepository<CornEntity>();
    private readonly IRepository<PlumbingEntity> _plumbRepo = factory.GetRepository<PlumbingEntity>();

    private readonly IEntityToReport<PlumbingEntity, ReportYeller> _plumbToR = plumbToR;
    private readonly IEntityToReport<CaliperEntity, ReportYeller> _caliperToR = caliperToR;
    private readonly IEntityToReport<CornEntity, ReportYeller> _cornToR = cornToR;

    private readonly IEntityToReport<CustardPlumbingLink, ReportYeller> _cpLinkToR = cpLinkToR;
    private readonly IEntityToReport<CustardCaliperLink, ReportYeller> _custCalToR = custCalToR;
    private readonly IEntityToReport<CustardCornLink, ReportYeller> _custCornToR = custCornToR;

    private readonly IYellerSettings _settings = settings;

    public async Task<Result<List<ReportYeller>>> TransformAsync(List<Plumbing> data)
    {
        //*********************************************************************************
        // Load All Relevant data
        //*********************************************************************************

        // Load all entities relevant to Yeller
        // These entities do not need navigation properties loaded
        Result<List<CaliperEntity>> calipers = await _caliperRepo.FindAsync(c => c.Source == _settings.YellerCaliperSource1 || c.Source == _settings.YellerCaliperSource2); // This source is a specific string
        if (calipers.IsFailure) return Result.Failure<List<ReportYeller>>(calipers.Error);

        Result<List<CornEntity>> corns = await _cornRepo.FindAsync(c => c.Source == _settings.YellerCornSource); // This source is a specific string
        if (corns.IsFailure) return Result.Failure<List<ReportYeller>>(corns.Error);

        Result<List<PlumbingEntity>> plumbs = await _plumbRepo.FindAsync(c => c.Source == Domain.ValueObjects.Source.Yeller); // This source is an enum with db conversion to string
        if (plumbs.IsFailure) return Result.Failure<List<ReportYeller>>(plumbs.Error);

        // Load CustardEntities --> getting them with details is less expensive than getting the details separately
        HashSet<long> plumbLookup = [.. data.Select(x => x.Id)];
        HashSet<long> caliperLookup = [.. calipers.Value.Select(x => x.Id)];
        HashSet<long> cornLookup = [.. corns.Value.Select(x => x.Id)];
        Result<List<CustardEntity>> custards = await _custardRepo.FindWithDetailsAsync(c =>
                c.CustardPlumbingLinks.Any(link => plumbLookup.Contains(link.PlumbingId)) ||
                c.CustardCaliperLinks.Any(link => caliperLookup.Contains(link.CaliperId)) ||
                c.CustardCornLinks.Any(link => cornLookup.Contains(link.CornId))
            ); // Retrieves RELEVANT custard entities instead of ALL custard entities
        if (custards.IsFailure) return Result.Failure<List<ReportYeller>>(custards.Error);

        //*********************************************************************************
        // Filter out entities that are associated with a custard,
        // in order to create a partition -> Custard/NonCustard entities
        //*********************************************************************************

        List<CustardCaliperLink> custardCalipers = [.. custards.Value.SelectMany(c => c.CustardCaliperLinks)];
        HashSet<long> custardCaliperIds = [.. custardCalipers.Select(c => c.CaliperId)];
        List<CaliperEntity> nonCustardCalipers = [.. calipers.Value.Where(c => !custardCaliperIds.Contains(c.Id))];

        List<CustardCornLink> custardCorns = [.. custards.Value.SelectMany(c => c.CustardCornLinks)];
        HashSet<long> custardCornIds = [.. custardCorns.Select(c => c.CornId)];
        List<CornEntity> nonCustardCorns = [.. corns.Value.Where(c => !custardCornIds.Contains(c.Id))];

        List<CustardPlumbingLink> custardPlumbs = [.. custards.Value.SelectMany(c => c.CustardPlumbingLinks)];
        HashSet<long> plumbingIds = [.. custardPlumbs.Select(d => d.PlumbingId)];
        List<PlumbingEntity> nonCustardPlumbs = [.. plumbs.Value.Where(c => !plumbingIds.Contains(c.Id))];


        //*********************************************************************************
        // Associations
        //*********************************************************************************

        // All entities associated with the same custard must be qualified based on date
        // The first entity attributable to a specific custard wins the value
        // To be attributable, the entity date must
        // 1. Be earlier than all custard dates, including sand dates, 
        // 2. Be earlier than all other entities

        // Build dictionaries for quick lookup
        Dictionary<long, CaliperEntity> caliperById = calipers.Value.ToDictionary(c => c.Id);
        Dictionary<long, CornEntity> cornById = corns.Value.ToDictionary(c => c.Id);
        Dictionary<long, PlumbingEntity> plumbById = plumbs.Value.ToDictionary(c => c.Id);

        // Flatten the relationship between a custard, <link>, and <entity>
        IEnumerable<CustardAssociation<CaliperEntity, CustardCaliperLink>> custardCaliperAssociations =
            from custard in custards.Value
            from link in custard.CustardCaliperLinks
            let caliper = caliperById[link.CaliperId]
            select new CustardAssociation<CaliperEntity, CustardCaliperLink>(link, caliper, custard, caliper.Date);

        IEnumerable<CustardAssociation<CornEntity, CustardCornLink>> custardCornAssociations =
            from custard in custards.Value
            from link in custard.CustardCornLinks
            let corn = cornById[link.CornId]
            select new CustardAssociation<CornEntity, CustardCornLink>(link, corn, custard, corn.Date);

        IEnumerable<CustardAssociation<PlumbingEntity, CustardPlumbingLink>> custardPlumbAssociations =
            from custard in custards.Value
            from link in custard.CustardPlumbingLinks
            let plumb = plumbById[link.PlumbingId]
            select new CustardAssociation<PlumbingEntity, CustardPlumbingLink>(link, plumb, custard, plumb.Date);

        // Attributable associations
        IEnumerable<CustardAssociation<CaliperEntity, CustardCaliperLink>> caliperAttributable = Attributable(custardCaliperAssociations);
        IEnumerable<CustardAssociation<CornEntity, CustardCornLink>> cornAttributable = Attributable(custardCornAssociations);
        IEnumerable<CustardAssociation<PlumbingEntity, CustardPlumbingLink>> plumbAttributable = Attributable(custardPlumbAssociations);

        // Non-attributable associations
        HashSet<long> caliperAttributableIds = [.. caliperAttributable.Select(a => a.Entity.Id)];
        HashSet<long> cornAttributableIds = [.. cornAttributable.Select(a => a.Entity.Id)];
        HashSet<long> plumbAttributableIds = [.. plumbAttributable.Select(a => a.Entity.Id)];

        //*********************************************************************************
        // Report
        //*********************************************************************************
        
        // Non-custard entities can now be translated into reports
        List<ReportYeller> nonCustardCaliperReports = [.. nonCustardCalipers.Select(_caliperToR.Translate)];
        List<ReportYeller> nonCustardCornReports = [.. nonCustardCorns.Select(_cornToR.Translate)];
        List<ReportYeller> nonCustardPlumbReports = [.. nonCustardPlumbs.Select(_plumbToR.Translate)];

        // Report non-attributable associations
        IEnumerable<ReportYeller> caliperNonAttributableReport =
            custardCaliperAssociations
                .Where(a => !caliperAttributableIds.Contains(a.Entity.Id))
                .Select(c => _caliperToR.Translate(c.Entity));
        IEnumerable<ReportYeller> cornNonAttributableReport =
            custardCornAssociations
                .Where(a => !cornAttributableIds.Contains(a.Entity.Id))
                .Select(c => _cornToR.Translate(c.Entity));
        IEnumerable<ReportYeller> plumbNonAttributableReport =
            custardPlumbAssociations
                .Where(a => !plumbAttributableIds.Contains(a.Entity.Id))
                .Select(c => _plumbToR.Translate(c.Entity));

        // Report attributable associations
        IEnumerable<ReportYeller> caliperAttributableReport = caliperAttributable
            .Select(c => _custCalToR.Translate(c.Link));
        IEnumerable<ReportYeller> cornAttributableReport = cornAttributable
            .Select(c => _custCornToR.Translate(c.Link));
        IEnumerable<ReportYeller> plumbAttributableReport = plumbAttributable
            .Select(c => _cpLinkToR.Translate(c.Link));

        // Gather reports
        List<ReportYeller> reports =
        [
            .. nonCustardCaliperReports,
            .. nonCustardCornReports,
            .. nonCustardPlumbReports,
            .. caliperNonAttributableReport,
            .. cornNonAttributableReport,
            .. plumbNonAttributableReport,
            .. caliperAttributableReport,
            .. cornAttributableReport,
            .. plumbAttributableReport
        ];

        return reports;

    }
    static IEnumerable<CustardAssociation<TEntity, TLink>> Attributable<TEntity, TLink>(IEnumerable<CustardAssociation<TEntity, TLink>> associations)
    {
        IEnumerable<CustardAssociation<TEntity, TLink>> result = associations
            .GroupBy(a => a.Custard.Id)
            .Select(g =>
            {
                var earliest = g.MinBy(a => a.EntityDate);
                return earliest is not null && IsAttributable(earliest)
                    ? earliest
                    : null;
            })
            .OfType<CustardAssociation<TEntity, TLink>>();
        return result;
    }

    static bool IsAttributable<T, TLink>(CustardAssociation<T, TLink> a)
    {
        IEnumerable<DateTime> cutoffDates =
            a.Custard.SandEntities.Select(s => s.Date)
            .Append(a.Custard.Date);
        bool result = a.EntityDate < cutoffDates.Min();
        return result;
    }
    record CustardAssociation<T, TLink>(TLink Link, T Entity, CustardEntity Custard, DateTime EntityDate);
}
