using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate;

internal class PlumbingEntityToPlumbing : IEntityToVo<PlumbingEntity, Plumbing>
{
    public Plumbing Translate(PlumbingEntity entity)
    {
        var number = new PhoneNumber(entity.PhoneNumber);
        DateTimeOffset date = new(entity.Date, TimeSpan.FromSeconds(0));
        var contents = entity.Contents;
        var source = entity.Source;

        var result = new Plumbing(PhoneNumber: number, Date: date, Contents: contents, Source: source);
        return result;
    }
}
