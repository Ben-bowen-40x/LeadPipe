using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Service;

[SourceKey(Source.Yeller)]
internal sealed class YellerUpdateService(
    IDataSourceAsync<YellerDto> source,
    ILoadData<YellerDto> load,
    IDtoToVo<YellerDto, Plumbing> dtoToVo,
    IVoToEntity<Plumbing, PlumbingEntity> voToEntity,
    IDataPersistence<PlumbingEntity> persistence
    ) : UpdateService<YellerDto, Plumbing, PlumbingEntity>(source, load, dtoToVo, voToEntity, persistence), IUpdateService<Plumbing>
{ }