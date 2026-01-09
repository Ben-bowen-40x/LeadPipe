using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Translation.Translate.EntityToReport;

internal sealed class PlumbingEntityToReportYeller : IEntityToReport<PlumbingEntity, ReportYeller>
{
    public ReportYeller Translate(PlumbingEntity data)
    {
        long eventtime = data.UnixDate;
        string eventName = "lead";
        string num = YellerReportHelper.HashSha256(data.PhoneNumber.ToString());
        string eventid = data.Id.ToString();
        
        UserData user = new()
        {
            ph = [num]
        };
        CustomData custom = new()
        {
            currency = YellerReportHelper.Currency,
            value = 0
        };

        ReportYeller result = new()
        {
            event_id = eventid,
            event_name = eventName,
            event_time = eventtime,
            custom_data = custom,
            user_data = user
        };
        return result;
    }
}