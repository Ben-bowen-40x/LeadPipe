using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public sealed class CalliManager([FromKeyedServices(Source.Calli)] IUpdateService<Plumbing> update) : UpdateManager(update), ICalliManager { }