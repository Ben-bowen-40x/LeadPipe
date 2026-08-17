using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Interface.Service;

namespace LeadPipe.Infrastructure.Service;

internal class SheetUploadService : IUploadService
{
    public Result UploadData<Row>(IList<Row> data, string sheetId, string tabName)
    {
        return Result.Failure("Not Implemented");
    }
}
