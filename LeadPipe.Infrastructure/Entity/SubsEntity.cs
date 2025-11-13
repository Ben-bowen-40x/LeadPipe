using LeadPipe.Application.DataInterfaces.Entity;

namespace LeadPipe.Infrastructure.Entity;

internal class SubsEntity : ISubsEntity
{
    public int Id { get; set; }
    public long PhoneNumber { get; set; }
    public DateTime CustDate { get; set; }
    public DateTime SubDate { get; set; }

    // Foreign keys for associations
    public long? LeafPhoneNumber { get; set; }
    public long? YellerPhoneNumber { get; set; }
    public long? CalliPhoneNumber { get; set; }
    public long? LabPhoneNumber { get; set; }

    // Navigation properties
    public ILeafEntity? LeafEntity { get; set; }
    public IYellerEntity? YellerEntity { get; set; }
    public ICalliEntity? CalliEntity { get; set; }
    public ILabEntity? LabEntity { get; set; }
}
