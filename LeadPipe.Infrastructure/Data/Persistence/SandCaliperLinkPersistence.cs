using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class SandCaliperLinkPersistence(IRepository<SandCaliperLink> repo) : Persistence<IRepository<SandCaliperLink>, SandCaliperLink>(repo), IDataPersistence<SandCaliperLink>{ }
