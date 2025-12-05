using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public interface IUpdateYellerManager : IUpdateManager { }
public sealed class UpdateYellerManager([FromKeyedServices(Source.Yeller)] IUpdateService<Plumbing> update) : UpdateManager(update), IUpdateYellerManager { }
