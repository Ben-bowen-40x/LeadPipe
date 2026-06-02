using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Database.Configuration;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interfaces.Repository;
using LeadPipe.Infrastructure.Sqlite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public sealed class CornRepository(
    PlumbingContext context,
    ILogger<CornRepository> logger
) : PlumbingContextEntityRepository<CornEntity, CornRepository>(context, logger), IRepository<CornEntity>
{
    protected override IQueryable<CornEntity> WithIncludes(IQueryable<CornEntity> q)
    {
        return q
            .Include(c => c.CustardCornLinks)
            .Include(c => c.SandCornLinks)
            .Include(c => c.CornCaliperLinks)
            .Include(c => c.CornPlumbingLinks);
    }

    protected override string? Type { get; set; } = "Corn";

    protected override UpsertFields EntityDetails { get; } =
    new(
        TableName: TableNames.CornEntitiesName,
        TempTable: $"temp_{TableNames.CornEntitiesName}",
        EntityName: nameof(CornEntity),
        ColumnCount: 13);

    protected override string CreateTempTable => $"""
        CREATE TEMP TABLE IF NOT EXISTS {EntityDetails.TempTable} (
            {nameof(CornEntity.Id)} INTEGER PRIMARY KEY,
            {nameof(CornEntity.PhoneNumber)} INTEGER NOT NULL,
            {nameof(CornEntity.Date)} TEXT NOT NULL,
            {nameof(CornEntity.UnixDate)} INTEGER NOT NULL,
            {nameof(CornEntity.Payload)} TEXT NOT NULL,
            {nameof(CornEntity.MetaData)} TEXT NOT NULL,
            {nameof(CornEntity.Source)} TEXT NOT NULL,
            {nameof(CornEntity.UtmSource)} TEXT,
            {nameof(CornEntity.UtmMedium)} TEXT,
            {nameof(CornEntity.UtmCampaign)} TEXT,
            {nameof(CornEntity.UtmContent)} TEXT,
            {nameof(CornEntity.UtmTerm)} TEXT,
            {nameof(CornEntity.ReferralSource)} TEXT
        ) WITHOUT ROWID;
        DELETE FROM {EntityDetails.TempTable};
    """;

    protected override string UpdateSql => $"""
        UPDATE {TableNames.CornEntitiesName}
        SET 
            {nameof(CornEntity.PhoneNumber)} = temp.{nameof(CornEntity.PhoneNumber)},
            {nameof(CornEntity.Date)} = temp.{nameof(CornEntity.Date)},
            {nameof(CornEntity.UnixDate)} = temp.{nameof(CornEntity.UnixDate)},
            {nameof(CornEntity.Payload)} = temp.{nameof(CornEntity.Payload)},
            {nameof(CornEntity.MetaData)} = temp.{nameof(CornEntity.MetaData)},
            {nameof(CornEntity.Source)} = temp.{nameof(CornEntity.Source)},
            {nameof(CornEntity.UtmSource)} = temp.{nameof(CornEntity.UtmSource)},
            {nameof(CornEntity.UtmMedium)} = temp.{nameof(CornEntity.UtmMedium)},
            {nameof(CornEntity.UtmCampaign)} = temp.{nameof(CornEntity.UtmCampaign)},
            {nameof(CornEntity.UtmContent)} = temp.{nameof(CornEntity.UtmContent)},
            {nameof(CornEntity.UtmTerm)} = temp.{nameof(CornEntity.UtmTerm)},
            {nameof(CornEntity.ReferralSource)} = temp.{nameof(CornEntity.ReferralSource)}
        FROM {EntityDetails.TempTable} temp
        WHERE {TableNames.CornEntitiesName}.{nameof(CornEntity.Id)} = temp.{nameof(CornEntity.Id)};
    """;

    protected override string InsertSql => $"""
        INSERT INTO {TableNames.CornEntitiesName} 
        (
            {nameof(CornEntity.Id)}, 
            {nameof(CornEntity.PhoneNumber)}, 
            {nameof(CornEntity.Date)}, 
            {nameof(CornEntity.UnixDate)}, 
            {nameof(CornEntity.Payload)}, 
            {nameof(CornEntity.MetaData)},  
            {nameof(CornEntity.Source)},
            {nameof(CornEntity.UtmSource)},
            {nameof(CornEntity.UtmMedium)},
            {nameof(CornEntity.UtmCampaign)},
            {nameof(CornEntity.UtmContent)},
            {nameof(CornEntity.UtmTerm)},
            {nameof(CornEntity.ReferralSource)}
        )
        SELECT 
            temp.{nameof(CornEntity.Id)}, 
            temp.{nameof(CornEntity.PhoneNumber)}, 
            temp.{nameof(CornEntity.Date)}, 
            temp.{nameof(CornEntity.UnixDate)}, 
            temp.{nameof(CornEntity.Payload)},     
            temp.{nameof(CornEntity.MetaData)}, 
            temp.{nameof(CornEntity.Source)},
            temp.{nameof(CornEntity.UtmSource)},
            temp.{nameof(CornEntity.UtmMedium)},
            temp.{nameof(CornEntity.UtmCampaign)},
            temp.{nameof(CornEntity.UtmContent)},
            temp.{nameof(CornEntity.UtmTerm)},
            temp.{nameof(CornEntity.ReferralSource)}
        FROM {EntityDetails.TempTable} temp
        WHERE NOT EXISTS (
            SELECT 1 
            FROM {TableNames.CornEntitiesName} t
            WHERE t.{nameof(CornEntity.Id)} = temp.{nameof(CornEntity.Id)}
        );
    """;

    protected override bool IsUpdatable => true;

    private static int[]? _columnIndexes;
    protected override int[] ColumnIndexes => _columnIndexes ??= [.. Enumerable.Range(0, EntityDetails.ColumnCount)];
    protected override void InsertBatch(List<CornEntity> batch)
    {
        var values = new List<object?>();
        var rows = new List<string>();

        for (int i = 0; i < batch.Count; i++)
        {
            var e = batch[i];
            int offset = i * EntityDetails.ColumnCount;

            var placeholders = ColumnIndexes.Select(ci => $"{{{offset + ci}}}");

            // Build placeholder string: ({0}, {1}, {2}, {3}, {4}, {5}, {6})
            rows.Add($"({string.Join(", ", placeholders)})");

            // Order here must match order below
            values.Add(e.Id);
            values.Add(e.PhoneNumber.Number);
            values.Add(e.Date.ToString(IsoString));
            values.Add(e.UnixDate);
            values.Add(e.Payload ?? string.Empty);
            values.Add(e.MetaData ?? string.Empty);
            values.Add(e.Source ?? string.Empty);
            values.Add(e.UtmSource); 
            values.Add(e.UtmMedium);
            values.Add(e.UtmCampaign);
            values.Add(e.UtmContent);
            values.Add(e.UtmTerm);
            values.Add(e.ReferralSource);
        }

        // Order here must match order above
        string sql = $"""
            INSERT INTO {EntityDetails.TempTable} (
                {nameof(CornEntity.Id)},
                {nameof(CornEntity.PhoneNumber)},
                {nameof(CornEntity.Date)},
                {nameof(CornEntity.UnixDate)},
                {nameof(CornEntity.Payload)},
                {nameof(CornEntity.MetaData)},
                {nameof(CornEntity.Source)},
                {nameof(CornEntity.UtmSource)},
                {nameof(CornEntity.UtmMedium)},
                {nameof(CornEntity.UtmCampaign)},
                {nameof(CornEntity.UtmContent)},
                {nameof(CornEntity.UtmTerm)},
                {nameof(CornEntity.ReferralSource)}
            )
            VALUES {string.Join(",", rows)};
            """;
        _context.Database.ExecuteSqlRaw(sql, [.. values]);
    }

    public override async Task<Result<List<CornEntity>>> UpsertRangeAsync(
        List<CornEntity> entities,
        CancellationToken ct = default) => await UpsertEntityRangeAsync(entities, ct);

}
