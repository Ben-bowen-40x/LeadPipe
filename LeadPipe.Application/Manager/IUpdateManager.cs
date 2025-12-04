using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Manager;

public interface IUpdateManager
{
    Task<Result<List<Plumbing>>> ManageAsync(bool update = true);
}
public interface ICalliManager : IUpdateManager { }
public interface ILabManager : IUpdateManager { }
public interface ILeafManager : IUpdateManager { }
public interface IYellerManager : IUpdateManager { }