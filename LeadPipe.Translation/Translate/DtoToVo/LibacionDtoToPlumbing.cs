using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interface.Translate;
using System.Text.RegularExpressions;

namespace LeadPipe.Translation.Translate.DtoToVo;

public sealed partial class LibacionDtoToPlumbing : IDtoToVo<LibacionDto, Plumbing>
{
    public Plumbing Translate(LibacionDto data)
    {
        PhoneNumber number = PhoneNumber.TryParse(data.PhoneNumber, out PhoneNumber r)
            ? r
            : PhoneNumber.DefaultPhoneNumber;
        DateTimeKind kind = DateTimeKind.Utc;

        // Parse time and date
        string time = string.IsNullOrWhiteSpace(data.Time) ?
            "" :
            GetTimeValue(data.Time);
        DateTime d = DateTime.TryParse($"{data.Date} {time}", out DateTime dt)
            ? DateTime.SpecifyKind(dt, kind)
            : DateTime.SpecifyKind(DateTime.MaxValue, kind);
        DateTimeOffset date = new(d, TimeSpan.Zero);

        string contents = data.Message is null
            ? string.Empty
            : data.Message;

        var isComm = data.Commercial is string c ? c : "Unknown";
        string metadata = $"Is Commercial: {isComm}";

        return new Plumbing(
            Id: 0,
            PhoneNumber: number,
            Date: date,
            Contents: contents,
            Branch: null,
            MetaData: metadata,
            Source.Libacion,
            [number]
            );
    }
    private static string GetTimeValue(string time)
    {
        Match m = TimePattern().Match(time);
        return m.Success ? m.Value : string.Empty;
    }
    [GeneratedRegex(@"\d{1,2}:\d{2}(:\d{2})?\s*((a|p)m)?", RegexOptions.IgnoreCase)]
    private static partial Regex TimePattern();
}