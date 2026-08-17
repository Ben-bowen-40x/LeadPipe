using CSharpFunctionalExtensions;
using LeadPipe.Infrastructure.Interface.Service;

namespace LeadPipe.Infrastructure.Service;

internal class SheetUploadService : ISheetUploadService
{
    public Result UploadData<Row>(FileInfo jsonCredentials, IList<Row> data, string sheetId, string tableName)
    {
        return Result.Failure("Not Implemented");
    }
    public Result UploadData<Row>(FileInfo jsonCredentials, IList<Row> data, Uri sheetUri, string tableName)
    {
        return Result.Failure("Not Implemented");
    }
}
