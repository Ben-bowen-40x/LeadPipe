using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Repository
{
    internal interface ICallRepository
    {
        Task<Result> AddAsync(CallEntity entity);
        Task<Result> AddRangeAsync(List<CallEntity> entities);
        Task<Result> DeleteAsync(CallEntity entity);
        Task<Result> DeleteAsync(long id);
        Task<Result<List<CallEntity>>> GetAllAsync();
        Task<Result<CallEntity>> GetAsync(CallEntity entity);
        Task<Result<CallEntity>> GetByIdAsync(long id);
        Task<Result> HardUpdateAsync(CallEntity entity);
        Task<Result> UpdateValuesAsync(CallEntity entity);
    }
}