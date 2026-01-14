using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;
using LeadPipe.Translation.Primitives;

namespace LeadPipe.Translation.Translate.EntityToVo;

internal class CustardMySqlEntityToCustard(IDateTimeTranslate dt) : IEntityToVo<CustardMySqlEntity, Custard>
{
    private readonly IDateTimeTranslate _dt = dt;
    public Custard Translate(CustardMySqlEntity entity)
    {
        PhoneNumber phone1 = PhoneNumber.TryParse(entity.phone1, out var p1) ? p1 : new PhoneNumber(PhoneNumber.Default);
        PhoneNumber phone2 = PhoneNumber.TryParse(entity.phone2, out var p2) ? p2 : new PhoneNumber(PhoneNumber.Default);

        DateTimeOffset date = _dt.Convert(entity.dateAdded, ETimeZone.Pacific);
        DateTimeOffset dateCancelled = _dt.Convert(entity.dateCancelled, ETimeZone.Pacific);

        bool status = entity.status == 1;

        Custard result = new
        (
            Id: entity.customerID,
            Status: status,
            Phone1: phone1,
            Phone2: phone2,
            Date: date,
            DateCancelled: dateCancelled
        );
        return result;
    }
}
internal sealed class CustardEntityToCustard : IEntityToVo<CustardEntity, Custard>
{
    public Custard Translate(CustardEntity entity)
    {
        PhoneNumber number1 = new(entity.PhoneNumber);
        PhoneNumber number2 = new(entity.PhoneNumber);
        
        DateTime d = DateTime.SpecifyKind(entity.Date, DateTimeKind.Utc);
        DateTimeOffset date = new(d, TimeSpan.Zero);
        
        DateTime cxl = DateTime.SpecifyKind(entity.CancelDate, DateTimeKind.Utc);
        DateTimeOffset cxlDate = new(cxl, TimeSpan.Zero);
        
        Custard result = new
            (
                Id: entity.Id,
                Status: entity.Active,
                Phone1: number1,
                Phone2: number2,
                Date: date,
                DateCancelled: cxlDate
            );
        return result;
    }
}