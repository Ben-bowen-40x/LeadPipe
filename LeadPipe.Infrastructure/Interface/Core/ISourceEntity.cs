namespace LeadPipe.Infrastructure.Interface.Core;

public interface ISourceEntity : IEntity
{
    Domain.ValueObjects.Source Source { get; set; }
}