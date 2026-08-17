using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class SandEntityPersistence(IRepository<SandEntity> repo)
    : Persistence<IRepository<SandEntity>, SandEntity>(repo), IDataPersistence<SandEntity>
{ }
