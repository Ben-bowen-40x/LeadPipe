using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate;

internal class PlumbingToPlumbingEntity : IVoToEntity<Plumbing, PlumbingEntity>
{
    public PlumbingEntity Translate(Plumbing plumbing)
    {
        var result = new PlumbingEntity()
        {
            PhoneNumber = plumbing.PhoneNumber.Number,
            Date = new(plumbing.Date.Ticks),
            UnixDate = plumbing.Date.ToUnixTimeSeconds(),
            Contents = plumbing.Contents,
            Source = plumbing.Source,
        };
        return result;
    }
}
