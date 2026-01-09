using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.MySql;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Service.Update;

internal sealed class SandwichUpdateService(
    IDataSourceAsync<SandMySqlEntity> subs,
    IEntityToVo<SandMySqlEntity, Sandwich> eToVo,
    IDataPersistence<Sandwich> persist
    ) : ValueObjectUpdateService<SandMySqlEntity, Sandwich>(subs, eToVo, persist), IUpdateService<Sandwich>
{ }