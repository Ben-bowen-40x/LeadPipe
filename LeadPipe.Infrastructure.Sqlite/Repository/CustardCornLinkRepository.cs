using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public sealed class CustardCornLinkRepository
    (
        PlumbingContext context,
        ILogger<CustardCornLinkRepository> logger
    ) : PlumbingContextLinkRepository<CustardCornLink, CustardCornLinkRepository>(context, logger), IRepository<CustardCornLink>
{
    protected override IQueryable<CustardCornLink> WithIncludes(IQueryable<CustardCornLink> q)
    {
        return q
            .Include(q => q.Custard)
            .Include(q => q.Corn);
    }

    protected override UpsertFields LinkDetails => new(
        TableName: TableNames.CustardCornLinksName,
        TempTable: $"temp_{TableNames.CustardCornLinksName}",
        Id1: nameof(CustardCornLink.CustardId),
        Id2: nameof(CustardCornLink.CornId),
        PhoneCol: nameof(CustardCornLink.MatchingPhone),
        DateCol: nameof(CustardCornLink.UnixMatchDate),
        EntityName: nameof(CustardCornLink)
    );
    protected override async Task AddLinks(List<CustardCornLink> links, int batchSize, CancellationToken ct)
    {
        for (int i = 0; i < links.Count; i += batchSize)
        {
            var batch = links.GetRange(i, Math.Min(batchSize, links.Count - i));
            var values = new List<object>();
            var rows = new List<string>();

            for (int j = 0; j < batch.Count; j++)
            {
                var link = batch[j];

                int o = j * 4;
                rows.Add($"({{{o}}}, {{{o + 1}}}, {{{o + 2}}}, {{{o + 3}}})");
                values.Add(link.CustardId);
                values.Add(link.CornId);
                values.Add(link.MatchingPhone);
                values.Add(link.UnixMatchDate);
            }

            string joined = $"INSERT INTO {LinkDetails.TempTable} VALUES {string.Join(",", rows)}";
            await _context.Database.ExecuteSqlRawAsync(joined, values, ct);
        }
    }

    private record ParentFields(string Parent1Name, string Parent1Id, string Parent2Name, string Parent2Id);
    private static ParentFields Parent => new(
        Parent1Name: TableNames.CustardEntitiesName,
        Parent1Id: nameof(CustardEntity.Id),
        Parent2Name: TableNames.CornEntitiesName,
        Parent2Id: nameof(CornEntity.Id)
    );
    protected override string InsertSql => $"""
        INSERT INTO {LinkDetails.TableName} 
        (
            {LinkDetails.Id1}, 
            {LinkDetails.Id2}, 
            {LinkDetails.PhoneCol}, 
            {LinkDetails.DateCol}
        )
        SELECT 
            t.{TempId1}, 
            t.{TempId2}, 
            (
                SELECT t2.{TempPhone}
                FROM {LinkDetails.TempTable} t2
                WHERE t2.{TempId1} = t.{TempId1}
                  AND t2.{TempId2} = t.{TempId2}
                  AND t2.{TempPhone} <> 0
                ORDER BY t2.{TempDate} ASC
                LIMIT 1
            ),
            MIN(t.{TempDate})
        FROM {LinkDetails.TempTable} t
        WHERE t.{TempPhone} <> 0
          AND NOT EXISTS (
              SELECT 1 
              FROM {LinkDetails.TableName} ccl
              WHERE ccl.{LinkDetails.Id1} = t.{TempId1} 
                AND ccl.{LinkDetails.Id2} = t.{TempId2}
          )
          AND EXISTS (
              SELECT 1 
              FROM {Parent.Parent1Name} c
              WHERE c.{Parent.Parent1Id} = t.{TempId1}
          )
          AND EXISTS (
              SELECT 1 
              FROM {Parent.Parent2Name} co
              WHERE co.{Parent.Parent2Id} = t.{TempId2}
          )
        GROUP BY t.{TempId1}, t.{TempId2};
    """;

    public override async Task<Result<List<CustardCornLink>>> UpsertRangeAsync(
        List<CustardCornLink> links,
        CancellationToken ct = default) => await UpsertLinkRangeAsync(links, ct);

}
