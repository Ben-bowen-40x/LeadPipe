namespace LeadPipe.Application.DataInterfaces.Entity;

public interface ILibacionEntity
{
    string? Contents { get; set; }
    DateTime Date { get; set; }
    long PhoneNumber { get; set; }
    long UnixDate { get; set; }
}
