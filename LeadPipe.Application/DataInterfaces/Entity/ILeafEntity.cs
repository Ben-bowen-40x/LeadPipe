namespace LeadPipe.Application.DataInterfaces.Entity;

public interface ILeafEntity
{
    string? Contents { get; set; }
    DateTime Date { get; set; }
    long PhoneNumber { get; set; }
    long UnixDate { get; set; }
}