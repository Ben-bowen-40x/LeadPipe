using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class CaliperEntityPersistence(IRepository<CaliperEntity> repo) : Persistence<IRepository<CaliperEntity>, CaliperEntity>(repo), IDataPersistence<CaliperEntity> { }
