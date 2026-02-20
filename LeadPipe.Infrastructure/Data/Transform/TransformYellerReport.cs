using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;
using LeadPipe.Infrastructure.Settings;

namespace LeadPipe.Infrastructure.Data.Transform;

internal sealed class TransformYellerReport(
    IRepositoryFactory factory,
    IEntityToReport<AttributionResult, ReportYeller> translate,
    IYellerSettings settings
    ) : ITransform<Plumbing, ReportYeller>
{

    private readonly IRepository<CustardEntity> _custardRepo = factory.GetRepository<CustardEntity>();
    private readonly IRepository<CaliperEntity> _caliperRepo = factory.GetRepository<CaliperEntity>();
    private readonly IRepository<CornEntity> _cornRepo = factory.GetRepository<CornEntity>();
    private readonly IRepository<PlumbingEntity> _plumbRepo = factory.GetRepository<PlumbingEntity>();

    private readonly IEntityToReport<AttributionResult, ReportYeller> _translate = translate;

    private readonly IYellerSettings _settings = settings;

    public async Task<Result<List<ReportYeller>>> TransformAsync(List<Plumbing> data)
    {
        //*********************************************************************************
        // Load All Relevant data
        //*********************************************************************************

        Result<List<CaliperEntity>> calipers =
            await _caliperRepo.FindAsync(c =>
                c.Source == _settings.YellerCaliperSource1 ||
                c.Source == _settings.YellerCaliperSource2);
        if (calipers.IsFailure) return Result.Failure<List<ReportYeller>>(calipers.Error);

        Result<List<CornEntity>> corns =
            await _cornRepo.FindAsync(c =>
                c.Source == _settings.YellerCornSource);
        if (corns.IsFailure) return Result.Failure<List<ReportYeller>>(corns.Error);

        Result<List<PlumbingEntity>> plumbs =
            await _plumbRepo.FindAsync(c =>
                c.Source == Domain.ValueObjects.Source.Yeller);
        if (plumbs.IsFailure) return Result.Failure<List<ReportYeller>>(plumbs.Error);

        HashSet<long> plumbLookup = data.Select(x => x.Id).ToHashSet();
        HashSet<long> caliperLookup = calipers.Value.Select(x => x.Id).ToHashSet();
        HashSet<long> cornLookup = corns.Value.Select(x => x.Id).ToHashSet();

        // Load custards with details
        Result<List<CustardEntity>> custards = await _custardRepo.FindWithDetailsAsync(c =>
                c.CustardPlumbingLinks.Any(link => plumbLookup.Contains(link.PlumbingId)) ||
                c.CustardCaliperLinks.Any(link => caliperLookup.Contains(link.CaliperId)) ||
                c.CustardCornLinks.Any(link => cornLookup.Contains(link.CornId))
            );
        if (custards.IsFailure) return Result.Failure<List<ReportYeller>>(custards.Error);

        //*********************************************************************************
        // Flatten sands: keep only first chronological sand per custard
        //*********************************************************************************

        // Any entity matching the custard is non-attributable when ANY sand or custard date is before the entity date
        custards = Result.Success(
            custards.Value
                .Where(c => c.SandEntities != null && c.SandEntities.Count != 0 && c.SandEntities.Any(s => s.Complete)) // filter out null/empty sands and remove any with all incomplete sands
                .Select(c =>
                {
                    // .OrderBy().First() = O(n log n)
                    // This is effectively O(n) + O(n)
                    long minDate = c.SandEntities
                        .Where(s => s.Complete)
                        .Min(s => s.UnixDate);
                    SandEntity firstSand = c.SandEntities
                        .First(s => s.UnixDate == minDate);
                    c.SandEntities = [firstSand]; // keep only the earliest sand 
                    return c;
                }).ToList()
        );

        //*********************************************************************************
        // Associations
        //*********************************************************************************

        Dictionary<long, CaliperEntity> caliperById = calipers.Value.ToDictionary(c => c.Id);
        Dictionary<long, CornEntity> cornById = corns.Value.ToDictionary(c => c.Id);
        Dictionary<long, PlumbingEntity> plumbById = plumbs.Value.ToDictionary(c => c.Id);

        var custardCaliperAssociations =
            from custard in custards.Value
            from link in custard.CustardCaliperLinks
            let caliper = caliperById[link.CaliperId]
            select new CustardAssociation<CaliperEntity, CustardCaliperLink>(link, caliper, custard, caliper.UnixDate);

        var custardCornAssociations =
            from custard in custards.Value
            from link in custard.CustardCornLinks
            let corn = cornById[link.CornId]
            select new CustardAssociation<CornEntity, CustardCornLink>(link, corn, custard, corn.UnixDate);

        var custardPlumbAssociations =
            from custard in custards.Value
            from link in custard.CustardPlumbingLinks
            let plumb = plumbById[link.PlumbingId]
            select new CustardAssociation<PlumbingEntity, CustardPlumbingLink>(link, plumb, custard, plumb.UnixDate);

        var caliperAttributable = Attributable(custardCaliperAssociations);
        var cornAttributable = Attributable(custardCornAssociations);
        var plumbAttributable = Attributable(custardPlumbAssociations);

        //*********************************************************************************
        // Cross-entity first-touch filter
        //*********************************************************************************

        IEnumerable<(long MatchingPhone, long UnixMatchDate, CustardEntity Custard)> allTouches =
        [
            .. plumbAttributable.Select(a => (a.Link.MatchingPhone, a.Link.UnixMatchDate, a.Custard)),
            .. cornAttributable.Select(a => (a.Link.MatchingPhone, a.Link.UnixMatchDate, a.Custard)),
            .. caliperAttributable.Select(a => (a.Link.MatchingPhone, a.Link.UnixMatchDate, a.Custard))
        ];

        // For each custard, earliest effective date = min(link.UnixMatchDate, custard.UnixDate, firstSandDate)
        IEnumerable<EffectiveDateAssociated> touchesWithEffectiveDate = allTouches.Select(t =>
        {
            long firstSandDate = t.Custard.SandEntities?.Min(s => s.UnixDate) ?? long.MaxValue;
            long effectiveDate = Math.Min(t.UnixMatchDate, Math.Min(firstSandDate, t.Custard.UnixDate));
            return new EffectiveDateAssociated(t.MatchingPhone, t.UnixMatchDate, t.Custard, EffectiveDate: effectiveDate);
        });

        Dictionary<long, List<CustardEntity>> firstTouchesByPhone = touchesWithEffectiveDate
            .GroupBy(t => t.MatchingPhone)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    long earliest = g.Min(t => t.EffectiveDate);
                    List<CustardEntity> result = [.. g.Where(t => t.EffectiveDate == earliest).Select(t => t.Custard)];
                    return result;
                });

        caliperAttributable =
            [.. caliperAttributable
                    .Where(a =>
                        firstTouchesByPhone.TryGetValue(a.Link.MatchingPhone, out List<CustardEntity>? custards) &&
                        custards.Contains(a.Custard) &&
                        a.Link.UnixMatchDate <= a.Custard.UnixDate)
            ];

        cornAttributable =
            [.. cornAttributable
                    .Where(a =>
                        firstTouchesByPhone.TryGetValue(a.Link.MatchingPhone, out List<CustardEntity>? custards) &&
                        custards.Contains(a.Custard) &&
                        a.Link.UnixMatchDate <= a.Custard.UnixDate)
            ];

        plumbAttributable =
            [.. plumbAttributable
                .Where(a =>
                    firstTouchesByPhone.TryGetValue(a.Link.MatchingPhone, out List<CustardEntity>? custards) &&
                    custards.Contains(a.Custard) &&
                    a.Link.UnixMatchDate <= a.Custard.UnixDate)
            ];

        //*********************************************************************************
        // Report
        //*********************************************************************************

        var caliperAttributionResults = caliperAttributable.Select(c =>
            new AttributionResult()
            {
                Custard = c.Custard,
                FirstTouchUnixDate = c.Link.UnixMatchDate,
                MatchingPhone = c.Link.MatchingPhone,
                Sand = c.Custard.SandEntities.Single(),
                Source = AttributionSource.Caliper
            });
        var cornAttributionResults = cornAttributable.Select(c =>
            new AttributionResult()
            {
                Custard = c.Custard,
                FirstTouchUnixDate = c.Link.UnixMatchDate,
                MatchingPhone = c.Link.MatchingPhone,
                Sand = c.Custard.SandEntities.Single(),
                Source = AttributionSource.Corn
            });
        var plumbAttributionResults = plumbAttributable.Select(c =>
            new AttributionResult()
            {
                Custard = c.Custard,
                FirstTouchUnixDate = c.Link.UnixMatchDate,
                MatchingPhone = c.Link.MatchingPhone,
                Sand = c.Custard.SandEntities.Single(),
                Source = AttributionSource.Plumbing
            });

        List<ReportYeller> reports =
        [
            .. caliperAttributionResults.Select(_translate.Translate),
            .. cornAttributionResults.Select(_translate.Translate),
            .. plumbAttributionResults.Select(_translate.Translate),
        ];

        return reports;
    }

    // Respect first sand after entity and Completed == true
    static IEnumerable<CustardAssociation<TEntity, TLink>> Attributable<TEntity, TLink>(IEnumerable<CustardAssociation<TEntity, TLink>> associations)
    {
        var result = associations
            .GroupBy(a => a.Custard.Id)
            .Select(g =>
            {
                var earliest = g.MinBy(a => a.EntityDate);
                return earliest != null && IsAttributable(earliest)
                    ? earliest
                    : null;
            })
            .OfType<CustardAssociation<TEntity, TLink>>();
        return result;
    }

    static bool IsAttributable<T, TLink>(CustardAssociation<T, TLink> a)
    {
        var sands = a.Custard.SandEntities; // already filtered to first sand
        if (sands == null || sands.Count == 0) return false; // Redundant check here isn't harmful

        var firstSandDate = sands.Min(s => s.UnixDate);

        // Entity must occur before custard AND before first sand
        return a.EntityDate < a.Custard.UnixDate && a.EntityDate < firstSandDate;
    }

    record CustardAssociation<T, TLink>(TLink Link, T Entity, CustardEntity Custard, long EntityDate);
    record EffectiveDateAssociated(long MatchingPhone, long UnixMatchDate, CustardEntity Custard, long EffectiveDate);
}

#region Logic map
/* No need to ask ai to help you understand what's going on!
Simply look at this diagram
                ┌─────────────────────────────┐
                │         ENTITIES            │
                │  Plumbing | Corn | Caliper  │
                └──────────────┬──────────────┘
                               │ (via link tables)
                               ▼
                ┌─────────────────────────────┐
                │          CUSTARDS           │
                │  1 custard can link to      │
                │  multiple entities          │
                └──────────────┬──────────────┘
                               │ (1 → many)
                               ▼
                ┌─────────────────────────────┐
                │            SANDS            │
                │  - Must be Complete         │
                │  - Only earliest counts     │
                └──────────────┬──────────────┘
                               │
                               ▼
                ┌─────────────────────────────┐
                │   PER-CUSTARD ATTRIBUTION   │
                │  - Earliest entity only     │
                │  - Entity < Custard         │
                │  - Entity < First Sand      │
                └──────────────┬──────────────┘
                               │
                               ▼
                ┌─────────────────────────────┐
                │ CROSS-ENTITY FIRST TOUCH    │
                │  Per Phone Number:          │
                │  - Compute Effective Date   │
                │  - Earliest wins            │
                │  - Tie → allow multiples    │
                └──────────────┬──────────────┘
                               │
                               ▼
                ┌─────────────────────────────┐
                │           REPORT            │
                │  - One winner per phone     │
                │  - Sand value attached      │
                │  - No double counting       │
                └─────────────────────────────┘

*/
#endregion