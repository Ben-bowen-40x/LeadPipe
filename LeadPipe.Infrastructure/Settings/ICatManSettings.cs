namespace LeadPipe.Infrastructure.Settings;

public interface ICatManSettings
{
    string? CatManClientName { get; set; }
    string? CatToken { get; set; }
    string? CatManDateFormat { get; set; }
    string? CatBaseEndpoint { get; set; }
    string? CatAccountId { get; set; }
}