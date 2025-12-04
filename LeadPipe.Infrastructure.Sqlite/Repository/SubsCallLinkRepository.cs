using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Repository;
using LeadPipe.Infrastructure.Sqlite.Context;

namespace LeadPipe.Infrastructure.Sqlite.Repository;

public class SubsCallLinkRepository(PlumbingContext context) : PlumbingContextRepository<CallSubsLink>(context), ISubsCallLinkRepository { }
