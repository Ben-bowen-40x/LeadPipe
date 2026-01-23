using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Application.Service;

public interface IUpdateFactory
{
    IUpdateService<T> GetService<T>(Source source);
    IUpdateService<T> GetService<T>();
}
