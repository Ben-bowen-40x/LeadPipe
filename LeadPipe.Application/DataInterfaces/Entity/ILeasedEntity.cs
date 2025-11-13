namespace LeadPipe.Application.DataInterfaces.Entity;

public interface ILeasedEntity
{
    string? Contents { get; set; }
    DateTime Date { get; set; }
    long PhoneNumber { get; set; }
    long UnixDate { get; set; }
}
