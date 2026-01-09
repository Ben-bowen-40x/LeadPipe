using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface ISandPlumbingLinkRepository : IRepository<SandPlumbingLink>
{
    public Task<Result<List<SandPlumbingLink>>> GetAllWithDetailsAsync();
    public Task<Result<List<SandPlumbingLink>>> GetAllWithDetailsAsync(IEnumerable<PlumbingEntity> filter);
    public Task<Result<List<SandPlumbingLink>>> GetAllAsync(IEnumerable<PlumbingEntity> filter);
}
