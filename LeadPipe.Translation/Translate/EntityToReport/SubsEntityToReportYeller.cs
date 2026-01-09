using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Translation.Translate.EntityToReport;

internal sealed class SubsEntityToReportYeller : IEntityToReport<SubsEntity, ReportYeller>
{
    public ReportYeller Translate(SubsEntity sub)
    {
        long eventTime = sub.UnixDate;
        UserData user = new()
        {
            ph = [sub.Number.ToString(), sub.Number2.ToString()],
        };
        CustomData custom = new()
        {
            currency = YellerReportHelper.Currency,
            value = sub.Value
        };
        string eventid = sub.Id.ToString();
        ReportYeller result = new() { event_id = eventid, event_time = eventTime, custom_data = custom, user_data = user };
        return result;
    }
}