using LeadPipe.Application.DataInterfaces.Entity;
namespace LeadPipe.Infrastructure.Entity;

internal class LeasedEntity : ILeasedEntity
{
    public long PhoneNumber { get; set; }
    public DateTime Date { get; set; }
    public long UnixDate { get; set; }
    public string? Contents { get; set; }
}
