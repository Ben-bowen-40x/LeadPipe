using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Service.Update;

internal sealed class SandwichUpdateService(
    IDataSourceAsync<SandMySqlEntity> sand,
    IEntityToVo<SandMySqlEntity, Sandwich> eToVo,
    IDataPersistence<Sandwich> persist
    ) : ValueObjectUpdateService<SandMySqlEntity, Sandwich>(sand, eToVo, persist, SyncKey.Sandwich), IUpdateService<Sandwich>
{ }
