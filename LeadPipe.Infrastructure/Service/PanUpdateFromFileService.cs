using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Service;

[SourceKey(Source.Pan)]
internal sealed class PanUpdateFromFileService(
    IDataSourceAsync<PanDto> source,
    ILoadData<PanDto> load,
    IDtoToVo<PanDto, Plumbing> dtoToVo,
    IVoToEntity<Plumbing, PlumbingEntity> voToEntity,
    IDataPersistence<PlumbingEntity> persistence
    ) : UpdateService<PanDto, Plumbing, PlumbingEntity>(source, load, dtoToVo, voToEntity, persistence), IUpdateService<Plumbing>
{ }
