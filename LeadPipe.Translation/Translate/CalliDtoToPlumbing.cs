using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate;

internal class CalliDtoToPlumbing : IDtoToVo<CalliDto, Plumbing>
{
    public Plumbing Translate(CalliDto v)
    {
        PhoneNumber phone = new(v.Phone);
        DateTime datetime = DateTime.TryParse(v.Date + " " + v.Time, out DateTime dt)
            ? dt
            : DateTime.MaxValue;
        DateTimeOffset date = datetime;

        return new Plumbing(PhoneNumber: phone, Date: date, Contents: v.PestProblem, Source: Source.Calli);
    }
}