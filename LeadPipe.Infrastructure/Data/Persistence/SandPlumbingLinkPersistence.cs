using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class SandPlumbingLinkPersistence(IRepository<SandPlumbingLink> repo) : Persistence<IRepository<SandPlumbingLink>, SandPlumbingLink>(repo), IDataPersistence<SandPlumbingLink>{ }
