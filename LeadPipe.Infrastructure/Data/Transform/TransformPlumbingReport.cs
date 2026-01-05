using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Transform;

public sealed class TransformPlumbingReport(
    ISubsPlumbingLinkRepository repo,
    IVoToEntity<Plumbing, PlumbingEntity> voToEntity,
    IEntityToReport<SubsPlumbingLink, ReportPlumbing> eToR
    ) : ITransform<Plumbing, ReportPlumbing>
{
    private readonly ISubsPlumbingLinkRepository _repo = repo;
    private readonly IVoToEntity<Plumbing, PlumbingEntity> _voToEntity = voToEntity;
    private readonly IEntityToReport<SubsPlumbingLink, ReportPlumbing> _eToR = eToR;
    public async Task<Result<List<ReportPlumbing>>> TransformAsync(List<Plumbing> data)
    {
        List<PlumbingEntity> e = [.. data.Select(_voToEntity.Translate)];
        Result<List<SubsPlumbingLink>> links = await _repo.GetAllWithDetailsAsync(e);
        List<SubsPlumbingLink>? entities = links.IsSuccess
            ? links.Value
            : null;
        if (entities is null)
            return Result.Failure<List<ReportPlumbing>>(links.Error);

        List<ReportPlumbing> result = [.. entities.Select(_eToR.Translate)];
        return result;
    }
}
