using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;

namespace LeadPipe.Infrastructure.Interfaces.Service;

public interface IYellerService
{
    Task<Result<List<YellerDto>>> GetAllAsync(bool refresh);
    Task<Result<List<YellerDto>>> RefreshAsync();
}
