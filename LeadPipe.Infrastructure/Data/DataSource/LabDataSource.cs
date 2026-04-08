using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Core;
using LeadPipe.Infrastructure.Interfaces.Service;

namespace LeadPipe.Infrastructure.Data.DataSource;

public class LabDataSource(ILabService lab) : IDataSourceAsync<LabDto>
{
    private readonly ILabService _lab = lab;
    public async Task<Result<List<LabDto>>> LoadAsync(bool _ = false)
    {
        Result<List<LabDto>> get = await _lab.GetLabsAsync();
        if (get.IsFailure)
            return Result.Failure<List<LabDto>>(get.Error);
        return get;
    }

    public async Task<Result<List<LabDto>>> RefreshAsync(bool _ = false)
    {
        Result<List<LabDto>> get = await _lab.UpdateDataAsync();
        if (get.IsFailure)
            return Result.Failure<List<LabDto>>(get.Error);
        return get;
    }
}
