namespace LeadPipe.Infrastructure.Interfaces.Core;

public interface IEntity
{
    long Id { get; set; }
}
public interface IHasUnixMatchDate
{
    long UnixMatchDate { get; set; }
}
