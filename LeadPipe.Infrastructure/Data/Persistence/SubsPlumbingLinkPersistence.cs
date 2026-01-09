using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class SubsPlumbingLinkPersistence(ISandPlumbingLinkRepository repo) : Persistence<ISandPlumbingLinkRepository, SandPlumbingLink>(repo), IDataPersistence<SandPlumbingLink>{ }
