using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Translation.Translate.VoToDto;

internal class PlumbingToLabDto : IVoToDto<Plumbing, LabDto>
{
    public LabDto Translate(Plumbing p)
    {
        throw new NotImplementedException();
    }
}
