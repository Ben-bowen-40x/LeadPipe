namespace LeadPipe.Infrastructure.Settings;

public interface IInfrastructureSettings :
    IDwhSettings,
    ILabSettings,
    ILeafSettings,
    IYellerSettings
{
    string? CaliperiReportLoc { get; set; }
    string? CaliperiSourceLoc { get; set; }

    string? LabReportLoc { get; set; }
    string? LabSourceLoc { get; set; }

    string? LibacionSourceLoc { get; set; }
    string? LibacionReportLoc { get; set; }

    string? LeasedSourceLoc { get; set; }
    string? LeasedReportLoc { get; set; }

    string? PanReportLoc { get; set; }
    string? PanSourceLoc { get; set; }
}
