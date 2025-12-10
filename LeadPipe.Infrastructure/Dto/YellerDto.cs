namespace LeadPipe.Infrastructure.Dto;

#pragma warning disable IDE1006 // Naming Styles
public class YellerDto
{
    public string? id { get; set; }
    public string? business_id { get; set; }
    public string? conversation_id { get; set; }
    public string? temporary_email_address { get; set; }
    public DateTime? temporary_email_address_expiry { get; set; }
    public string? temporary_phone_number { get; set; }
    public DateTime? time_created { get; set; }
    public DateTime? last_event_time { get; set; }
    public object? user { get; set; }
    public Project? project { get; set; }

}

#region Attached
public class Project
{
    public Location? location { get; set; }
    public Availability? availability { get; set; }
    public string[]? job_names { get; set; }
    public SurveyAnswer[]? survey_answers { get; set; }
    public Attach[]? attachments { get; set; }
}
public class Attach
{
    public string? id { get; set; }
    public string? url { get; set; }
    public string? resource_name { get; set; }
    public string? mime_type { get; set; }
}
public class SurveyAnswer
{
    public string? question_text { get; set; }
    public string? question_identifier { get; set; }
    public string[]? answer_text { get; set; }
}
public class Location
{
    public string? postal_code { get; set; }
}
public class Availability
{
    public string? status { get; set; }
    public string[]? dates { get; set; }
}
public class YellerHelperDto
{
    public string[]? lead_ids { get; set; }
    public bool has_more { get; set; }
}
#endregion

#pragma warning restore IDE1006 // Naming Styles