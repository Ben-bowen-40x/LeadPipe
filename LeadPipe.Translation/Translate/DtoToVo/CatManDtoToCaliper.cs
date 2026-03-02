using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Translation.Translate.DtoToVo;

internal sealed class CatManDtoToCaliper : IDtoToVo<CatManDto, Caliper>
{
    public Caliper Translate(CatManDto data)
    {
        long id = data.Id;
        
        long unix = (long)(data.UnixTime is null ? 0 : data.UnixTime);
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unix);
        
        PhoneNumber number = new(data.CallerNumberBare);
        
        double dur = (double)(data.Duration is null ? 0 : data.Duration);
        TimeSpan duration = TimeSpan.FromSeconds(dur);
        
        var location = data.Location!;
        var note = data.Form;
        string source = data.TrackingLabel!;
        bool billable = false;
        
        Caliper result = new(
            Id: id,
            Date: date,
            Number: number,
            Duration: duration,
            Note: note,
            Source: source,
            Billable: billable,
            Location: location
        );
        
        return result;
    }
}