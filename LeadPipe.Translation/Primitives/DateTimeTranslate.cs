namespace LeadPipe.Translation.Primitives;

public class DateTimeTranslate : IDateTimeTranslate
{
    // Cache TimeZoneInfo objects for efficiency
    private static readonly Dictionary<ETimeZone, TimeZoneInfo> TimeZones = new()
    {
        [ETimeZone.Pacific] = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
        [ETimeZone.Mountain] = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"),
        [ETimeZone.Central] = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"),
        [ETimeZone.Eastern] = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
    };

    #region Public

    /// <summary>
    /// Convert a local DateTime in the given ETimeZone to UTC DateTimeOffset.
    /// Handles invalid and ambiguous times deterministically.
    /// </summary>
    public DateTimeOffset Convert(DateTime localTime, ETimeZone zone)
    {
        if (localTime.Kind == DateTimeKind.Utc)
            return new DateTimeOffset(localTime, TimeSpan.Zero);

        if (!TimeZones.TryGetValue(zone, out var tz))
            tz = TimeZoneInfo.Utc;

        // Handle invalid times (DST gaps) by moving forward until valid
        if (tz.IsInvalidTime(localTime))
        {
            localTime = AdjustForwardToValid(localTime, tz);
        }

        // Handle ambiguous times (DST fall-back repeated hour)
        if (tz.IsAmbiguousTime(localTime))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localTime);

            // Choose policy: pick the **earlier offset** (usually DST)
            TimeSpan chosenOffset = offsets.Min();
            return new DateTimeOffset(localTime, chosenOffset).ToUniversalTime();
        }

        // Normal conversion
        return TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
    }

    /// <summary>
    /// Convert a local DateTime to UTC using out parameter.
    /// Returns false if conversion fails.
    /// </summary>
    public bool Convert(DateTime localTime, ETimeZone zone, out DateTimeOffset result)
    {
        try
        {
            result = Convert(localTime, zone);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Convert a DateTime to DateTimeOffset with the specified offset.
    /// </summary>
    public DateTimeOffset Convert(DateTime date, TimeSpan offset) => new(date, offset);

    #endregion

    #region Private Helpers

    /// <summary>
    /// Adjusts an invalid local time forward until it becomes valid.
    /// </summary>
    private static DateTime AdjustForwardToValid(DateTime date, TimeZoneInfo tz)
    {
        DateTime adjusted = date;
        while (tz.IsInvalidTime(adjusted))
            adjusted = adjusted.AddMinutes(1);
        return adjusted;
    }

    #endregion
}
