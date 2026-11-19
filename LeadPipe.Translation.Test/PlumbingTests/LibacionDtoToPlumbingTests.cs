using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Translation.Translate.DtoToVo;

namespace LeadPipe.Translation.Test.PlumbingTests;

public class LibacionDtoToPlumbingTests
{
    private readonly LibacionDtoToPlumbing _translator = new();

    [Fact]
    public void Translate_ValidDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        string testContents = "Test Contents";
        long phonenumber = 6555550142;
        string commercial = "";
        var dto = new LibacionDto
        {
            Date = "10/31/2026",
            Time = "16:45",
            PhoneNumber = $"{phonenumber}",
            TimeZone = "UTC",
            Message = testContents,
            Commercial = commercial
        };

        // Act
        Plumbing result = _translator.Translate(dto);

        // Assert
        Assert.Equal(0, result.Id);
        Assert.Equal(testContents, result.Contents);
        Assert.Null(result.Branch);
        Assert.Equal($"Is Commercial: {commercial}", result.MetaData);
        Assert.Equal(Source.Libacion, result.Source);

        Assert.Equal(
            DateTimeOffset.Parse("2026-10-31T16:45:00Z"),
            result.Date
        );

        Assert.Equal(phonenumber, result.PhoneNumber.Number);
    }

}
