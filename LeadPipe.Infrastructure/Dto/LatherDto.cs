using CsvHelper.Configuration.Attributes;

namespace LeadPipe.Infrastructure.Dto;

public class LatherDto
{
    public string? Phone { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    [Name("Time Zone")]
    public string? TimeZone { get; set; }
    [Name("LeadID")]
    public string? LeadId { get; set; }
}
