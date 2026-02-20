using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity;
using LeadPipe.Infrastructure.Interfaces.Translate;

namespace LeadPipe.Translation.Translate.EntityToReport;

internal sealed class AttributionResultToReportYeller : IEntityToReport<AttributionResult, ReportYeller>
{
    public ReportYeller Translate(AttributionResult result)
    {
        var custard = result.Custard;

        long eventtime = result.FirstTouchUnixDate;
        string eventname = "purchase";
        string eventid = custard.Id.ToString();

        string num1 = YellerReportHelper.HashSha256(
            custard.PhoneNumber.Number.ToString());

        string n2 = custard.PhoneNumber2 is null
            ? PhoneNumber.Default.ToString()
            : custard.PhoneNumber2.Number.ToString();

        string num2 = YellerReportHelper.HashSha256(n2);

        UserData user = new() { ph = [num1, num2] };

        CustomData custom = new()
        {
            currency = YellerReportHelper.Currency,
            value = result.Value
        };

        return new ReportYeller
        {
            event_id = eventid,
            event_name = eventname,
            event_time = eventtime,
            custom_data = custom,
            user_data = user
        };
    }

}