using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Repository
{
    internal interface ISubsCallLinkRepository
    {
        Task<Result> AddAsync(SubsCallLink entity);
        Task<Result> AddRangeAsync(List<SubsCallLink> entities);
        Task<Result> DeleteAsync(long id);
        Task<Result> DeleteAsync(SubsCallLink entity);
        Task<Result<List<SubsCallLink>>> GetAllAsync();
        Task<Result<SubsCallLink>> GetAsync(SubsCallLink entity);
        Task<Result<SubsCallLink>> GetByIdAsync(long id);
        Task<Result> HardUpdateAsync(SubsCallLink entity);
        Task<Result> UpdateValuesAsync(SubsCallLink entity);
    }
}