using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Dto;

namespace LeadPipe.Infrastructure.Interfaces.Service;

public interface ICatManService
{
    Task<Result<List<CatmanDto>>> GetAllAsync(DateTime start, DateTime end);
}