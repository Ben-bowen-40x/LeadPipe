using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Translation.Primitives;

namespace LeadPipe.Infrastructure.Translate;

internal class EntityToVo(IDateTimeTranslate dtranslate) : IEntityToVo
{
    private readonly IDateTimeTranslate _dt = dtranslate;
    public Plumbing Translate(PlumbingEntity entity)
    {
        var number = new PhoneNumber(entity.PhoneNumber);
        DateTimeOffset date = entity.Date;
        var contents = entity.Contents;
        var source = entity.Source;

        var result = new Plumbing(PhoneNumber: number, Date: date, Contents: contents, Source: source);
        return result;
    }

    public Sandwich Translate(SubsEntity entity)
    {
        long subId = entity.Id;
        long custId = entity.CustomerId;
        DateTimeOffset date = _dt.Convert(entity.Date, ETimeZone.Pacific);
        DateTimeOffset subDate = _dt.Convert(entity.SubDate, ETimeZone.Pacific);
        PhoneNumber num1 = new(entity.Number);
        PhoneNumber num2 = new(entity.Number2);
        DateTimeOffset cxlDate = _dt.Convert(entity.CancelDate, ETimeZone.Pacific);
        DateTimeOffset subCxlDate = _dt.Convert(entity.SubCancelDate, ETimeZone.Pacific);
        bool active = entity.Active;
        bool subActive = entity.SubActive;
        bool complete = entity.Complete;
        double cv = entity.Value;
        string? seller = entity.Seller;
        string? seller2 = entity.Seller2;
        string? seller3 = entity.Seller3;
        var result = new Sandwich()
        {
            SubscriptionId = subId,
            CustomerId = custId,
            Date = date,
            SubDate = subDate,
            Number = num1,
            Number2 = num2,
            CancelDate = cxlDate,
            SubCancelDate = subCxlDate,
            Active = active,
            SubActive = subActive,
            Complete = complete,
            Value = cv,
            Seller = seller,
            Seller2 = seller2,
            Seller3 = seller3,

        };
        return result;
    }
}
