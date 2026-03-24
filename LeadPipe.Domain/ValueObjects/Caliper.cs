namespace LeadPipe.Domain.ValueObjects;

public record Caliper(long Id, DateTimeOffset Date, PhoneNumber Number, TimeSpan Duration, string Note, string Source, string Label, bool Billable, string Location);
