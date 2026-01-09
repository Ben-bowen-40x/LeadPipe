namespace LeadPipe.Infrastructure.Entity.Sqlite;

public class SandCaliperLink : IEntity
{
    public long Id { get; set; }
    public long SandId { get; set; }
    public SandEntity? SandEntity { get; set; }

    public long CaliperId { get; set; }
    public CaliperEntity? CaliperEntity { get; set; }

    public long MatchingNumber { get; set; }
}
