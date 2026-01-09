using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface ISandCornLinkRepository : IRepository<SandCornLink>
{
    Task<Result<List<SandCornLink>>> GetAllWithDetailsAsync();
    Task<Result<List<SandCornLink>>> GetAllWithDetailsAsync(IEnumerable<CornEntity> filter);
    Task<Result<List<SandCornLink>>> GetAllAsync(IEnumerable<CornEntity> filter);
}