using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public sealed class LeafManager([FromKeyedServices(Source.Leaf)] IUpdateService<Plumbing> update) : UpdateManager(update), ILeafManager { }