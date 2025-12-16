namespace LeadPipe.Domain.ValueObjects;

public record Plumbing(PhoneNumber PhoneNumber, DateTimeOffset Date, string? Contents, string MetaData, Source Source);
