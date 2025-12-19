using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.MySql;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class SubMySqlEntityPersistence(
    ISubsRepository repo,
    IEntityToVo<SubMySqlEntity, Sandwich> eToVo,
    IVoToEntity<Sandwich, SubsEntity> voToE
    ) : MySqlEntityPersistence<ISubsRepository, SubsEntity, SubMySqlEntity, Sandwich>(repo, eToVo, voToE), IDataPersistence<SubMySqlEntity>
{ }
