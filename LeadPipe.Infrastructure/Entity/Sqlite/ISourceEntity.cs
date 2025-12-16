namespace LeadPipe.Infrastructure.Entity.Sqlite;

public interface ISourceEntity : IEntity
{
    Domain.ValueObjects.Source Source { get; set; }
}