using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Service;

[SourceKey(Source.Lab)]
internal sealed class LabUpdateService(
    IDataSourceAsync<LabDto> source,
    ILoadData<LabDto> load,
    IDtoToVo<LabDto, Plumbing> dtoToVo,
    IVoToEntity<Plumbing, PlumbingEntity> voToEntity,
    IDataPersistence<PlumbingEntity> persistence
    ) : UpdateService<LabDto, Plumbing, PlumbingEntity>(source, load, dtoToVo, voToEntity, persistence), IUpdateService<Plumbing>
{ }
