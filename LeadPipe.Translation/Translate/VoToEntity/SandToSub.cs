using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Translate;

namespace LeadPipe.Translation.Translate.VoToEntity;

internal class SandToSub : IVoToEntity<Sandwich, SubsEntity>
{
    public SubsEntity Translate(Sandwich s)
    {
        var result = new SubsEntity()
        {
            Id = s.SubscriptionId,
            CustomerId = s.CustomerId,
            Date = new(s.Date.Ticks),
            UnixDate = s.Date.ToUnixTimeSeconds(),
            SubDate = new(s.SubDate.Ticks),
            UnixSubDate = s.SubDate.ToUnixTimeSeconds(),
            Number = s.Number.Number,
            Number2 = s.Number2.Number,
            CancelDate = new(s.CancelDate.Ticks),
            UnixCancelDate = s.CancelDate.ToUnixTimeSeconds(),
            SubCancelDate = new(s.SubCancelDate.Ticks),
            UnixSubCancelDate = s.SubCancelDate.ToUnixTimeSeconds(),
            Active = s.Active,
            SubActive = s.SubActive,
            Complete = s.Complete,
            Value = s.Value,
            Seller = s.Seller,
            Seller2 = s.Seller2,
            Seller3 = s.Seller3
        };
        return result;
    }
}
