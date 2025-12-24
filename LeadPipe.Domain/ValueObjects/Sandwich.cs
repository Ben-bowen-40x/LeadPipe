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
    int Seller, 
    int Seller2, 
    int Seller3);
