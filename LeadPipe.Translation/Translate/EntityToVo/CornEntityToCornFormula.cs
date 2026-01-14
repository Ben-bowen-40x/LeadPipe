using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Translation.Translate.EntityToVo;

internal sealed class CornEntityToCornFormula : IEntityToVo<CornEntity, CornFormula>
{
    public CornFormula Translate(CornEntity entity)
    {
        PhoneNumber phoneNumber = new(entity.PhoneNumber);
        DateTime d = DateTime.SpecifyKind(entity.Date, DateTimeKind.Utc);
        DateTimeOffset date = new(d, TimeSpan.Zero);
        CornFormula result = new
            (
                Id: entity.Id,
                PhoneNumber: phoneNumber,
                Date: date,
                PayLoad: entity.Payload,
                MetaData: entity.MetaData,
                Source: entity.Source
            );
        return result;
    }
}