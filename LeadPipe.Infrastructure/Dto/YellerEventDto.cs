namespace LeadPipe.Infrastructure.Dto;

#pragma warning disable IDE1006 // Naming Styles

public class YellerEventDto
{
    public Event[]? events { get; set; }
}

#region Helpers
public class Event
{
    public string? id { get; set; }
    public string? cursor { get; set; }
    public DateTime? time_created { get; set; }
    public string? event_type { get; set; }
    public string? user_type { get; set; }
    public Event_Content? event_content { get; set; }
    public string? user_id { get; set; }
    public string? user_display_name { get; set; }
}

public class Event_Content
{
    public string? fallback_text { get; set; }
    public string? text { get; set; }
}
#endregion

#pragma warning restore IDE1006 // Naming Styles