namespace LeadPipe.Infrastructure.Settings;

public interface ICatManSettings
{
    string? CatManClientName { get; set; }
    string? CatToken { get; set; }
    string? CatManDateFormat { get; set; }
    string? CatBaseEndpoint { get; set; }
    string? CatmanSecret { get; set; }
    string? CatmanKey { get; set; }
    CatAccountId? CatAccountId { get; set; }
}
public class CatAccountId
{
    public int Fat { get; set; }
    public int Sandbox { get; set; }
    public int Natal { get; set; }
}