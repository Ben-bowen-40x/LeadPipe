using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Infrastructure.Factory;

class UpdateSourceFactory(IServiceProvider provider) : IUpdateFactory
{
    private readonly IServiceProvider _provider = provider;
    public IUpdateService<T> GetService<T>(Source source)
    {
        return _provider.GetRequiredKeyedService<IUpdateService<T>>(source);
    }
    public IUpdateService<T> GetService<T>()
    {
        return _provider.GetRequiredService<IUpdateService<T>>();
    }
}
