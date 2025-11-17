using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Translate;

public interface IEntityToVo
{
    Plumbing Translate(PlumbingEntity entity);
    Sandwich Translate(SubsEntity entity);
}