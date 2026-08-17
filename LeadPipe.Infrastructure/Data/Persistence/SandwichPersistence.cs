using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class SandwichPersistence(
    IDataPersistence<SandEntity> persist,
    IVoToEntity<Sandwich, SandEntity> voToE
    ) : VoPersistence<SandEntity, Sandwich>(persist, voToE), IDataPersistence<Sandwich>
{ }
