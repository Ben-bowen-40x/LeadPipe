using System.Runtime.InteropServices;

namespace LeadPipe.Infrastructure.Entity.Sqlite;

public class SubscriptionEntity : IEntity
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public required CustomerEntity Customer { get; set; }

    public DateTime StartDate { get; set; }
    public long UnixSubDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public long UnixCancelDate { get; set; }
    public bool Active { get; set; }

    public string? Type { get; set; }
    public double ContractValue { get; set; }

    public string? Seller { get; set; }
    public string? Seller2 { get; set; }
    public string? Seller3 { get; set; }

    public ICollection<SubsPlumbingLink> SubsPlumbingLinks { get; set; } = [];
    public ICollection<SubsCallLink> SubsCallLinks { get; set; } = [];
}
