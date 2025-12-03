using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Translate;
using LeadPipe.Translation.Primitives;

namespace LeadPipe.Translation.Translate;

internal class LabDtoToPlumbing : IDtoToVo<LabDto, Plumbing>
{
    public Plumbing Translate(LabDto dto)
    {
        throw new NotImplementedException();
    }
}
