using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Service;
using LeadPipe.Infrastructure.Interface.Translate;

namespace LeadPipe.Infrastructure.Data.DataSource;

public class LeafDataSource(ILeafService leaf, IVoToDto<Plumbing, LeafDto> voToDto) : IDataSourceAsync<LeafDto>
{
    private readonly ILeafService _leaf = leaf;
    private readonly IVoToDto<Plumbing, LeafDto> _voToDto = voToDto;
    public async Task<Result<List<LeafDto>>> LoadAsync(bool _ = false)
    {
        Result<List<Plumbing>> get = await _leaf.GetAllAsync();
        if (get.IsFailure)
            return Result.Failure<List<LeafDto>>(get.Error);
        List<LeafDto> result = [.. get.Value.Select(_voToDto.Translate)];
        return result;
    }

    public async Task<Result<List<LeafDto>>> RefreshAsync(bool _ = false)
    {
        Result<List<Plumbing>> get = await _leaf.RefreshAsync();
        if (get.IsFailure)
            return Result.Failure<List<LeafDto>>(get.Error);
        List<LeafDto> result = [.. get.Value.Select(_voToDto.Translate)];
        return result;
    }
}
