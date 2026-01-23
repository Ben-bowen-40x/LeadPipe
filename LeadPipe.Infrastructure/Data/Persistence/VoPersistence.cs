using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal abstract class VoPersistence<TEntity, TVo>(
    IDataPersistence<TEntity> persist,
    IVoToEntity<TVo, TEntity> voToE
    ) : IDataPersistence<TVo>
{
    private readonly IDataPersistence<TEntity> _persist = persist;
    private readonly IVoToEntity<TVo, TEntity> _voToE = voToE;
    public async Task<Result> SaveAsync(List<TVo> t)
    {
        List<TEntity> entities = [.. t.Select(_voToE.Translate)];
        Result result = await _persist.SaveAsync(entities);
        return result;
    }
}