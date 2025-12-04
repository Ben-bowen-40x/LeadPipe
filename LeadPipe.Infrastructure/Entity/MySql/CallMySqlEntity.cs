namespace LeadPipe.Infrastructure.Entity.MySql;

public class CallMySqlEntity
{
#pragma warning disable IDE1006 // Naming Styles
    public long call_id { get; set; }
    public int duration { get; set; }
    public string? sale_billable { get; set; }
    public string? contact_number_clean { get; set; }
    public DateTime called_at_utc { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}
