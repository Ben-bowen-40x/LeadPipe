using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interface.Core;
using LeadPipe.Infrastructure.Interface.Service;

namespace LeadPipe.Infrastructure.Data.DataSource;

public class YellerDataSource(IYellerService yeller) : IDataSourceAsync<YellerDto>
{
    private readonly IYellerService _yeller = yeller;

    public async Task<Result<List<YellerDto>>> LoadAsync(bool _)
    {
        Result<List<YellerDto>> get = await _yeller.GetAllAsync(false);
        if (get.IsFailure)
            return Result.Failure<List<YellerDto>>(get.Error);

        return get;
    }

    public async Task<Result<List<YellerDto>>> RefreshAsync(bool _)
    {
        Result<List<YellerDto>> get = await _yeller.RefreshAsync();
        if (get.IsFailure)
            return Result.Failure<List<YellerDto>>(get.Error);

        return get;
    }
}
