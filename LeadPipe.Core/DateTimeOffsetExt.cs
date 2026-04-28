namespace LeadPipe.Core;

public static class DateTimeOffsetExt
{
    public static long ToUnixTime(this DateTimeOffset dto)
    {
        return dto.ToUnixTimeMilliseconds();
    }
    public static DateTimeOffset FromUnixTime(this long l)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(l);
    }
}
