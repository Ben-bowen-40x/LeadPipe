using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;
using System.Text.RegularExpressions;

namespace LeadPipe.Translation.Translate.EntityToReport;

internal partial class SandPlumbingLinkToReportPlumbing : IEntityToReport<SandPlumbingLink, ReportPlumbing>
{
    private const string dateFormat = "yyyy-MM-dd HH:mm:ss";
    public ReportPlumbing Translate(SandPlumbingLink link)
    {
        if (link.PlumbingEntity is not PlumbingEntity plumb)
            throw new Exception("Plumbing entities cannot be null");

        if (link.SandEntity is not SandEntity sub)
            sub = new()
            {
                CustardId = 0,
                Id = 0,
                Active = false,
                Complete = false,
                Date = DateTime.MinValue,
                CancelDate = DateTime.MinValue,
                Offerman = string.Empty
            };

        long phoneNumber = plumb.PhoneNumber.Number;

        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(plumb.UnixDate);
        string formattedDate = date.ToString(dateFormat);

        string message = plumb.Contents is string c 
            ? NewLineRegex().Replace(c.Replace(',', ';')," | ") 
            : string.Empty;
        string source = plumb.Source.ToString();
        string metadata = plumb.MetaData;

        long customerId = sub.CustardId;
        long subId = sub.Id;
        bool subActive = sub.Active;
        bool completed = sub.Complete;

        // Dates
        DateTimeOffset custDate = DateTimeOffset.FromUnixTimeSeconds(sub.UnixDate);
        string formattedCustDate = custDate.ToString(dateFormat);

        DateTimeOffset custCxlDate = DateTimeOffset.FromUnixTimeSeconds(sub.UnixCancelDate);

        DateTimeOffset subDate = DateTimeOffset.FromUnixTimeSeconds(sub.UnixDate);
        string formattedSubDate = subDate.ToString(dateFormat);

        DateTimeOffset subCxlDate = DateTimeOffset.FromUnixTimeSeconds(sub.UnixCancelDate);

        bool msgBeforeCust = date < custDate && date < subDate;
        bool isSale = msgBeforeCust && completed;

        ReportPlumbing result = new()
        {
            MsgBeforeCust = msgBeforeCust,
            IsSale = isSale,
            PhoneNumber = phoneNumber,
            Date = date,
            FormattedDate = formattedDate,
            Message = message,
            Source = source,
            MetaData = metadata,
            CustomerId = customerId,
            SubId = subId,
            SubActive = subActive,
            Completed = completed,
            CustDate = custDate,
            FormattedCustDate = formattedCustDate,
            CustCxlDate = custCxlDate,
            SubDate = subDate,
            FormattedSubDate = formattedSubDate,
            SubCxlDate = subCxlDate,
        };
        return result;
    }

    [GeneratedRegex(@"(\s*(\n\r|\r\n|\n|\r)\s*)+")]
    private static partial Regex NewLineRegex();
}
