using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;

namespace LeadPipe.Infrastructure.Interface.Service;

public interface ILabService
{
    Task<Result<List<LabDto>>> GetLabsAsync(int errorLimit = 5, CancellationToken ct = default);
    Task<Result<List<LabDto>>> UpdateDataAsync(int errorLimit = 5, CancellationToken ct = default);
}
