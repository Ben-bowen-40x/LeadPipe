namespace LeadPipe.Domain.ValueObjects;

public record Customer(
    long Id,
    PhoneNumber Phone1,
    PhoneNumber Phone2,
    DateTimeOffset Created,
    DateTimeOffset? Cancelled,
    bool Active
);
