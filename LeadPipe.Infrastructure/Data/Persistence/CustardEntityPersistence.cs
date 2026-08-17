using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class CustardEntityPersistence(IRepository<CustardEntity> repo)
    : Persistence<IRepository<CustardEntity>, CustardEntity>(repo), IDataPersistence<CustardEntity>
{ }