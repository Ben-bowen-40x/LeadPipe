namespace LeadPipe.Infrastructure.Entity.Sqlite;

public class CustomerEntity : IEntity
{
    public long Id { get; set; }
    public long Phone1 { get; set; }
    public long Phone2 { get; set; }
    public DateTime DateAdded { get; set; }
    public long UnixDateAdded { get; set; }
    public DateTime CancelDate { get; set; }
    public long UnixCancelDate { get; set; }
    public bool Active { get; set; }

    public ICollection<SubscriptionEntity> Subscriptions { get; set; } = [];
}
