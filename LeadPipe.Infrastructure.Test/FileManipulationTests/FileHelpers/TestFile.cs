using CsvHelper.Configuration;

namespace LeadPipe.Infrastructure.Test.FileManipulationTests.FileHelpers;

internal class TestFile : ClassMap<TestFile>
{
    public TestFile()
    {
        string id = nameof(Id);
        string name = nameof(Name);
        string dateTime = nameof(DateTime);
        int index = 0;
        Map(m => m.Id).Index(index++).Name(id);
        Map(m => m.Name).Index(index++).Name(name);
        Map(m => m.DateTime).Index(index++).Name(dateTime);
    }
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime DateTime { get; set; }
}
