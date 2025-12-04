namespace LeadPipe.Domain.ValueObjects;

public record Subscription(
    long Id,
    long CustomerId,
    DateTimeOffset StartDate,
    DateTimeOffset? CancelDate,
    bool Active,
    string? ServiceType,
    double ContractValue,
    string? Seller,
    string? Seller2,
    string? Seller3
);
