using LeadPipe.Application.DataInterfaces.Entity;
namespace LeadPipe.Infrastructure.Entity;

internal class LibacionEntity : ILibacionEntity
{
    public long PhoneNumber { get; set; }
    public DateTime Date { get; set; }
    public long UnixDate { get; set; }
    public string? Contents { get; set; }
}
