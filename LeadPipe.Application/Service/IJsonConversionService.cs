using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace LeadPipe.Application.Service;

public interface IJsonConversionService
{
    Result<List<T>> Extract<T>(FileInfo jsonFile);
    Result<FileInfo> SaveToCsv<T, TMap>(Result<List<T>> entities, FileInfo csvFile) where TMap : ClassMap<T>;
}
