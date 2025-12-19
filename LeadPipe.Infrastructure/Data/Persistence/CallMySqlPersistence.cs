using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class CallMySqlPersistence(
    ICallRepository repo,
    IEntityToVo<CallMySqlEntity, Call> eToVo,
    IVoToEntity<Call, CallEntity> callToE
    ) : MySqlEntityPersistence<ICallRepository, CallEntity, CallMySqlEntity, Call>(repo, eToVo, callToE), IDataPersistence<CallMySqlEntity>
{ }
