namespace LeadPipe.Infrastructure.Dto;

public class Token
{
    public string Access_token { get; set; } = null!;
    public string Token_type { get; set; } = null!;
    public string? Refresh_token { get; set; }
    public int Expires_in { get; set; }
}
