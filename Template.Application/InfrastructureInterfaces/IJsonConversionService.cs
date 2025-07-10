using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Template.Application.InfrastructureInterfaces;

public interface IJsonConversionService
{
    Result<List<T>> Extract<T>(FileInfo jsonFile);
    Result<FileInfo> SaveToCsv<T, TMap>(Result<List<T>> entities, FileInfo csvFile) where TMap : ClassMap<T>;
}
