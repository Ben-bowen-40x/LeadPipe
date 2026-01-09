using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity.Sqlite;

namespace LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;

public interface ISandCaliperLinkRepository : IRepository<SandCaliperLink>
{
    Task<Result<List<SandCaliperLink>>> GetAllWithDetailsAsync();
    Task<Result<List<SandCaliperLink>>> GetAllWithDetailsAsync(List<CaliperEntity> list);
}
