using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Service.Update;

internal sealed class CalipersUpdateService(
    IDataSourceAsync<CaliperMySqlEntity> call,
    IEntityToVo<CaliperMySqlEntity, Caliper> eToVo,
    IDataPersistence<Caliper> persistence
    ) : ValueObjectUpdateService<CaliperMySqlEntity, Caliper>(call, eToVo, persistence, SyncKey.Caliper), IUpdateService<Caliper>
{ }

