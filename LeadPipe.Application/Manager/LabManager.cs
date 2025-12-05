using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public interface ILabManager : IUpdateManager { }
public sealed class LabManager([FromKeyedServices(Source.Lab)] IUpdateService<Plumbing> update) : UpdateManager(update), ILabManager { }
