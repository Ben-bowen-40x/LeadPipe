using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal abstract class MySqlEntityPersistence<TRepo, TEntity, TMySqlEntity, TVo>(
        TRepo repo,
        IEntityToVo<TMySqlEntity, TVo> eToVo,
        IVoToEntity<TVo, TEntity> voToE
        ) : IDataPersistence<TMySqlEntity>
    where TEntity : class, IEntity
    where TRepo : IRepository<TEntity>
{
    private readonly TRepo _repo = repo;
    private readonly IEntityToVo<TMySqlEntity, TVo> _eToVo = eToVo;
    private readonly IVoToEntity<TVo, TEntity> _voToE = voToE;
    public async Task<Result> SaveAsync(List<TMySqlEntity> t)
    {
        List<TEntity> translation = [.. t.Select(v => _voToE.Translate(_eToVo.Translate(v)))];
        Result<List<TEntity>> added = await _repo.UpsertRangeAsync(translation);
        return added;
    }
}