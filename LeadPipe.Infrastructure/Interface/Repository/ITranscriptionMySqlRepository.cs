using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Entity;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Interface.Repository;

public interface ITranscriptionMySqlRepository
{
    Task<Result<List<TranscriptionMySqlEntity>>> FindAsync(Expression<Func<TranscriptionMySqlEntity, bool>> predicate);
    Task<Result<TranscriptionMySqlEntity>> GetByIdAsync(long callId);
}