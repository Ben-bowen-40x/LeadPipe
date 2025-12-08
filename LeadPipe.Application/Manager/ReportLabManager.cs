using LeadPipe.Application.Service;
using LeadPipe.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Application.Manager;

public interface IReportLabManager : IReportManager<Plumbing> { }
public sealed class ReportLabManager([FromKeyedServices(Source.Lab)] IReportService<Plumbing> report) : ReportManager<Plumbing>(report), IReportLabManager { }
