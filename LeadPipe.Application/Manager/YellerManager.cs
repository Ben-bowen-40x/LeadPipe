using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public interface IYellerManager : IUpdateManager { }
public sealed class YellerManager([FromKeyedServices(Source.Yeller)] IUpdateService<Plumbing> update) : UpdateManager(update), IYellerManager { }
