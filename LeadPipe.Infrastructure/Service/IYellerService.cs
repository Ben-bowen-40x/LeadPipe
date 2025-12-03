using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Infrastructure.Service;

public interface IYellerService
{
    Task<Result<List<Plumbing>>> GetAllAsync();
    Task<Result<List<Plumbing>>> RefreshAsync();
}
