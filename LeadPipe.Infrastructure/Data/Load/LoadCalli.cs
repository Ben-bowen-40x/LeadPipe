using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Attribute;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Repository;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Data.Load;

[SourceKey(Source.Calli)]
public sealed class LoadCalli(IRepository<PlumbingEntity> repo, IEntityToVo<PlumbingEntity, Plumbing> eToVo) :
    LoadData<Plumbing, PlumbingEntity>(repo, eToVo, Source.Calli)
{ }
