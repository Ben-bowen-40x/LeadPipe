using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class CustardPersistence(
    IDataPersistence<CustardEntity> persist,
    IVoToEntity<Custard, CustardEntity> voToE
    ) : VoPersistence<CustardEntity, Custard>(persist, voToE), IDataPersistence<Custard>
{ }
