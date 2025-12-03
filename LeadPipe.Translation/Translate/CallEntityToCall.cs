using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate;

internal class CallEntityToCall : IEntityToVo<CallEntity, Call>
{
    public Call Translate(CallEntity c)
    {
        DateTimeOffset date = new(c.CallDate, TimeSpan.FromSeconds(0));
        PhoneNumber number = new(c.PhoneNumber);
        TimeSpan duration = TimeSpan.FromSeconds(c.Duration);
        string note = c.Note;
        string source = c.Source;
        bool billable = c.Billable;

        var result = new Call(Date: date, Number: number, Duration: duration, Note: note, Source: source, Billable: billable);
        return result;
    }
}
