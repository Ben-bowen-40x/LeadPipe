using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Translate;

internal class VoToEntity : IVoToEntity
{
    public SubsEntity Translate(Sandwich s)
    {
        long id = s.SubscriptionId;
        long customerId = s.CustomerId;
        DateTime date = new DateTime(s.Date.Ticks);
        long unixDate = s.Date.ToUnixTimeSeconds();
        DateTime subDate = new DateTime(s.SubDate.Ticks);
        long unixSubDate = s.SubDate.ToUnixTimeSeconds();
        long number = s.Number.Number;
        long number2 = s.Number2.Number;
        DateTime cancelDate = new DateTime(s.CancelDate.Ticks);
        long unixCancelDate = s.CancelDate.Ticks;
        DateTime subCancelDate = new DateTime(s.SubCancelDate.Ticks);
        long unixSubCancelDate = s.SubCancelDate.Ticks;
        bool active = s.Active;
        bool subActive = s.SubActive;
        bool complete = s.Complete;
        double value = s.Value;
        string? seller = s.Seller;
        string? seller2 = s.Seller2;
        string? seller3 = s.Seller3;
        var result = new SubsEntity()
        {
            Id = id,
            CustomerId = customerId,
            Date = date,
            UnixDate = unixDate,
            SubDate = subDate,
            UnixSubDate = unixSubDate,
            Number = number,
            Number2 = number2,
            CancelDate = cancelDate,
            UnixCancelDate = unixCancelDate,
            SubCancelDate = subCancelDate,
            UnixSubCancelDate = unixSubCancelDate,
            Active = active,
            SubActive = subActive,
            Complete = complete,
            Value = value,
            Seller = seller,
            Seller2 = seller2,
            Seller3 = seller3,

        };
        return result;
    }

    public PlumbingEntity Translate(Plumbing plumbing)
    {
        throw new NotImplementedException();
    }
}
