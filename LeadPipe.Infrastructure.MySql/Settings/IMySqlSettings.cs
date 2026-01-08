namespace LeadPipe.Infrastructure.MySql.Settings;

public interface IMySqlSettings
{
    public string? Schema1ConnectionString { get; set; }
    public string? Schema2ConnectionString { get; set; }
    public string? Schema3ConnectionString { get; set; }
    public string? Schema1 { get; set; }
    public string? Schema2 { get; set; }
    public string? Schema3 { get; set; }
    public string? CornTableName { get; set; }
}
