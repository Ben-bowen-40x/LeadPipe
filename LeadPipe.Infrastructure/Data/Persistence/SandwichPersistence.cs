using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal sealed class SandwichPersistence(
    IDataPersistence<SandEntity> persist,
    IVoToEntity<Sandwich, SandEntity> voToE
    ) : IDataPersistence<Sandwich>
{
    private readonly IDataPersistence<SandEntity> _persist = persist;
    private readonly IVoToEntity<Sandwich, SandEntity> _voToE = voToE;
    public async Task<Result> SaveAsync(List<Sandwich> t)
    {
        List<SandEntity> entities = [.. t.Select(_voToE.Translate)];
        Result result = await _persist.SaveAsync(entities);
        return result;
    }
}