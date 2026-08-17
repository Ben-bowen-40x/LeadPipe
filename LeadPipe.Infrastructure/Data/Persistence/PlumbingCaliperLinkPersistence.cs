using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Repository;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class PlumbingCaliperLinkPersistence(IRepository<PlumbingCaliperLink> repo) 
    : Persistence<IRepository<PlumbingCaliperLink>, PlumbingCaliperLink>(repo), IDataPersistence<PlumbingCaliperLink>{ }
