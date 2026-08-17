using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Translation.Primitives;

internal class PlumbingMetaDataCanonicalPersistenceFormat : IPlumbingMetaDataCanonicalPersistenceFormat<PlumbingEntity, string>
{
    public string Translate(PlumbingEntity t) => t.MetaData is null ? string.Empty : t.MetaData;
}
