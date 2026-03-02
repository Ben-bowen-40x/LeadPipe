using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Manager;

public interface ICatManManager
{
    Task<Result> Manage(bool refresh);
}
internal class CatManManager(IUpdateFactory factory) : ICatManManager
{
    private readonly IUpdateService<Caliper> _catMan = factory.GetService<Caliper>(Source.Yeller);
    public async Task<Result> Manage(bool refresh)
    {
        var result = refresh
            ? await _catMan.UpdateDataAsync(false)
            : await _catMan.GetDataAsync(false);
        return result;
    }
}
