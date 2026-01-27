using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Service;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Infrastructure.Data.Source;

public class YellerDataSource(IYellerService yeller, IVoToDto<Plumbing, YellerDto> voToDto) : IDataSourceAsync<YellerDto>
{
    private readonly IYellerService _yeller = yeller;
    private readonly IVoToDto<Plumbing, YellerDto> _voToDto = voToDto;

    public async Task<Result<List<YellerDto>>> LoadAsync()
    {
        Result<List<YellerDto>> get = await _yeller.GetAllAsync(false);
        if (get.IsFailure)
            return Result.Failure<List<YellerDto>>(get.Error);
        return get;
    }

    public async Task<Result<List<YellerDto>>> RefreshAsync()
    {
        Result<List<YellerDto>> get = await _yeller.RefreshAsync();
        if (get.IsFailure)
            return Result.Failure<List<YellerDto>>(get.Error);
        return get;
    }
}
