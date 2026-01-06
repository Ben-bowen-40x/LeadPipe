using CSharpFunctionalExtensions;

namespace LeadPipe.Application.Service;

public interface IPlumbingAssociationService
{
    Task<Result> SaveLinksAsync();
}
