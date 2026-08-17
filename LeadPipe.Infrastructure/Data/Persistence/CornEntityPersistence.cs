using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class CornEntityPersistence(IRepository<CornEntity> repo): Persistence<IRepository<CornEntity>, CornEntity>(repo), IDataPersistence<CornEntity> { }