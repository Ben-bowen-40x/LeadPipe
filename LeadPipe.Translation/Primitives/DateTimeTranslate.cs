namespace LeadPipe.Translation.Primitives;

internal class DateTimeTranslate : IDateTimeTranslate
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
    /// Handles invalid times deterministically.
    /// </summary>
    public DateTimeOffset Convert(DateTime localTime, ETimeZone zone)
    {
        if (localTime.Kind == DateTimeKind.Utc)
            return new DateTimeOffset(localTime, TimeSpan.Zero);

        if (!TimeZones.TryGetValue(zone, out var tz))
            tz = TimeZoneInfo.Utc;

        // Push invalid time into the next valid time slot
        if (tz.IsInvalidTime(localTime))
            localTime = PushIntoValidTime(localTime, tz);

        return TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
    }

    /// <summary>
    /// Convert a local DateTime in the given ETimeZone to UTC DateTimeOffset (out variant).
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
    /// Deterministically pushes an invalid time into the next valid time slot.
    /// </summary>
    private static DateTime PushIntoValidTime(DateTime date, TimeZoneInfo tz)
    {
        // Determine smallest increment for adjustment
        TimeSpan increment = TimeSpan.FromMinutes(1);
        DateTime adjusted = date;

        // Move forward until the time is valid
        while (tz.IsInvalidTime(adjusted))
            adjusted = adjusted.Add(increment);

        return adjusted;
    }

    #endregion
}

