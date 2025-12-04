using CSharpFunctionalExtensions;
using LeadPipe.Application.Service;

namespace LeadPipe.Application.Manager;

public interface IJsonManager
{
    Result<List<T>> ManageRead<T>(FileInfo file);
    Result ManageWrite<T>(FileInfo file, List<T> data);
}

public sealed class JsonManager(IFileRWService service) : IJsonManager
{
    private readonly IFileRWService _service = service;
    private const string error = "File extension not supported";
    public Result<List<T>> ManageRead<T>(FileInfo file)
    {
        return file.Extension switch
        {
            ".csv" => _service.ExtractCsv<T>(file),
            ".json" => _service.ExtractJson<T>(file),
            _ => Result.Failure<List<T>>(error)
        };
    }
    public Result ManageWrite<T>(FileInfo file, List<T> data)
    {
        return file.Extension switch
        {
            ".csv" => _service.SaveToCsv<T>(data, file),
            ".json" => _service.SaveToJson<T>(data, file),
            _ => Result.Failure(error)
        };
    }
}