namespace LeadPipe.Infrastructure.Entity.Sqlite;

public class CornCaliperLink : IEntity
{
    public long Id { get; set; }

    public long CornId { get; set; }
    public CornEntity CornEntity { get; set; } = default!;

    public long CaliperId { get; set; }
    public CaliperEntity CaliperEntity { get; set; } = default!;

    public long MatchingPhone { get; set; }
}
