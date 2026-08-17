using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Attribute;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Service.Update;

[SourceKey(Source.Calli)]
internal sealed class CalliUpdateFromFileService(
    IDataSourceAsync<CalliDto> source,
    IDtoToVo<CalliDto, Plumbing> dtoToVo,
    IVoToEntity<Plumbing, PlumbingEntity> voToEntity,
    IDataPersistence<PlumbingEntity> persistence
    ) : UpdateService<CalliDto, Plumbing, PlumbingEntity>(source, dtoToVo, voToEntity, persistence, SyncKey.Plumbing), IUpdateService<Plumbing>
{ }
