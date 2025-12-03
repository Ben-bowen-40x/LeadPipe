using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate;

internal class PlumbingToLabDto : IVoToDto<Plumbing, LabDto>
{
    public LabDto Translate(Plumbing p)
    {
        throw new NotImplementedException();
    }
}
