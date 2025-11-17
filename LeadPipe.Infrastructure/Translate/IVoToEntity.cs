using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Translate;

public interface IVoToEntity
{
    internal SubsEntity Translate(Sandwich s);
    internal PlumbingEntity Translate(Plumbing plumbing);
}