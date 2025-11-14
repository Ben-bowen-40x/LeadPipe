namespace LeadPipe.Infrastructure.Entity;

internal class SubsEntity
{
    public int Id { get; set; }
    public long PhoneNumber { get; set; }
    public DateTime CustDate { get; set; }
    public DateTime SubDate { get; set; }

    // Foreign keys for associations
    public long? CalliPhoneNumber { get; set; }
    public long? LabPhoneNumber { get; set; }
    public long? LeasedPhoneNumber { get; set; }
    public long? LeafPhoneNumber { get; set; }
    public long? LibacionPhoneNumber { get; set; }
    public long? PanPhoneNumber { get; set; }
    public long? YellerPhoneNumber { get; set; }

    // Navigation properties
    public ICollection<PlumbingEntity> PlumbingEntities { get; set; } = [];
}
