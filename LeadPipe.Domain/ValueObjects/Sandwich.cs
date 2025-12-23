namespace LeadPipe.Domain.ValueObjects;

public record Sandwich(
    long SubscriptionId, 
    long CustomerId, 
    DateTimeOffset Date, 
    DateTimeOffset SubDate, 
    PhoneNumber Number, 
    PhoneNumber Number2, 
    DateTimeOffset CancelDate, 
    DateTimeOffset SubCancelDate, 
    bool Active, 
    bool SubActive, 
    bool Complete, 
    string? Type, 
    decimal Value, 
    string? Seller, 
    string? Seller2, 
    string? Seller3);
