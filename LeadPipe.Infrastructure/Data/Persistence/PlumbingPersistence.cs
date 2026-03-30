using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

namespace LeadPipe.Infrastructure.Data.Persistence;

internal class PlumbingPersistence(
    IRepository<PlumbingEntity> plumbing,
    IRepository<PlumbingPhoneNumber> phone
    ) : IDataPersistence<PlumbingEntity>
{
    private readonly IRepository<PlumbingEntity> _plumbing = plumbing;
    private readonly IRepository<PlumbingPhoneNumber> _phone = phone;
    public async Task<Result> SaveAsync(List<PlumbingEntity> t)
    {
        var phonesToUpsert = t
            .Where(p => p.PhoneNumbers is not null && p.PhoneNumbers.Count > 0)
            .SelectMany(p => p.PhoneNumbers)
            .ToList();

        var upsertedPlumbingEntities = await _plumbing.UpsertRangeAsync(t);
        if (upsertedPlumbingEntities.IsFailure || phonesToUpsert.Count == 0) // If there are no phones to upsert, no need to proceed.
            return upsertedPlumbingEntities;

        var upsertedPhoneNumbers = await _phone.UpsertRangeAsync(phonesToUpsert);
        if (upsertedPhoneNumbers.IsFailure)
            return Result.Failure($"{nameof(PlumbingEntity)} list was upserted, but {nameof(PlumbingPhoneNumber)} failed to upsert: {upsertedPhoneNumbers.Error}");

        return Result.Success();
    }
}
