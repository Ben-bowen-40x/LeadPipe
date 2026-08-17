using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Attribute;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Service.Update;

[SourceKey(Source.Yeller)]
internal sealed class CatManUpdateService(
    IDataSourceAsync<CatManDto> source,
    IDtoToVo<CatManDto, CornFormula> dtoToVo,
    IVoToEntity<CornFormula, CornEntity> voToEntity,
    IDataPersistence<CornEntity> persistence
    ) : UpdateService<CatManDto, CornFormula, CornEntity>(source, dtoToVo, voToEntity, persistence, SyncKey.CornFormula), IUpdateService<CornFormula>
{ }