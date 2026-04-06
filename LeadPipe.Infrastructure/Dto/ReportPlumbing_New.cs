using CsvHelper.Configuration.Attributes;

namespace LeadPipe.Infrastructure.Dto;

internal sealed class ReportPlumbing_New
{
    [Name(ReportPlumbingColumnNames.PhoneNumberName)]
    public long PhoneNumber { get; set; }
    [Name(ReportPlumbingColumnNames.DateName)]
    public DateTimeOffset Date { get; set; }
    [Name(ReportPlumbingColumnNames.ContentsName)]
    public required string Contents { get; set; }
    [Name(ReportPlumbingColumnNames.SourceName)]
    public required string Source { get; set; }
    [Name(ReportPlumbingColumnNames.IMLName)]
    public bool IML { get; set; }
    [Name(ReportPlumbingColumnNames.ISLName)]
    public bool ISL { get; set; }
    [Name(ReportPlumbingColumnNames.CustardIdName)]
    public long CustardId { get; set; }
    [Name(ReportPlumbingColumnNames.SandActiveName)]
    public bool? SandActive { get; set; }
    [Name(ReportPlumbingColumnNames.CustardDateName)]
    public DateTimeOffset? CustardDate { get; set; }
    [Name(ReportPlumbingColumnNames.CustardCxlDateName)]
    public DateTimeOffset? CustardCxlDate { get; set; }
    [Name(ReportPlumbingColumnNames.SandIdName)]
    public long SandId { get; set; }
    [Name(ReportPlumbingColumnNames.CompletedName)]
    public bool Completed { get; set; }
    [Name(ReportPlumbingColumnNames.ValueName)]
    public decimal Value { get; set; }
    [Name(ReportPlumbingColumnNames.SandDateName)]
    public DateTimeOffset? SandDate { get; set; }
    [Name(ReportPlumbingColumnNames.SandCxlDateName)]
    public DateTimeOffset? SandCxlDate { get; set; }
    [Name(ReportPlumbingColumnNames.SellersName)]
    public string? Sellers { get; set; }
    [Name(ReportPlumbingColumnNames.TypeName)]
    public string? Type { get; set; }

}
