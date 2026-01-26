namespace LeadPipe.Infrastructure.Entity.Sqlite;

public class SyncStateEntity
{
    public int Id { get; set; }
    public required string BusinessId { get; set; }
    public string? LastProcessedId { get; set; }
    public DateTime LastSyncUtc { get; set; }
    public long UnixLastSyncUtc { get; set; }
}
