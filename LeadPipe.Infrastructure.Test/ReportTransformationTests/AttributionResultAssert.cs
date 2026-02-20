using LeadPipe.Infrastructure.Entity;

namespace LeadPipe.Infrastructure.Test.ReportTransformationTests;

public static class AttributionResultAssert
{
    public static void Equivalent(
        AttributionResult expected,
        AttributionResult actual)
    {
        Assert.NotNull(actual);

        Assert.Equal(expected.Custard.Id, actual.Custard.Id);
        Assert.Equal(expected.MatchingPhone, actual.MatchingPhone);
        Assert.Equal(expected.FirstTouchUnixDate, actual.FirstTouchUnixDate);
        Assert.Equal(expected.Source, actual.Source);

        if (expected.Sand is null)
        {
            Assert.Null(actual.Sand);
        }
        else
        {
            Assert.NotNull(actual.Sand);
            Assert.Equal(expected.Sand.Id, actual.Sand.Id);
            Assert.Equal(expected.Sand.UnixDate, actual.Sand.UnixDate);
            Assert.Equal(expected.Sand.Value, actual.Sand.Value);
        }
    }
}
