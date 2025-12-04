namespace LeadPipe.Translation.Test;

using LeadPipe.Translation.Primitives;
using System;
using Xunit;

public class DateTimeTranslateTests
{
    private readonly DateTimeTranslate _translator = new();

    private static readonly ETimeZone[] Zones = new[]
    {
        ETimeZone.Pacific,
        ETimeZone.Mountain,
        ETimeZone.Central,
        ETimeZone.Eastern
    };

    [Fact]
    public void UtcDateTime_ShouldRemainUnchanged()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTimeOffset result = _translator.Convert(utcNow, ETimeZone.Pacific);

        Assert.Equal(utcNow, result.UtcDateTime);
        Assert.Equal(TimeSpan.Zero, result.Offset);
    }

    [Fact]
    public void StandardLocalToUtcConversion_ShouldBeCorrect()
    {
        foreach (var zone in Zones)
        {
            DateTime local = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);
            DateTimeOffset utc = _translator.Convert(local, zone);

            TimeSpan expectedOffset = zone switch
            {
                ETimeZone.Pacific => TimeSpan.FromHours(-7), // DST in June
                ETimeZone.Mountain => TimeSpan.FromHours(-6),
                ETimeZone.Central => TimeSpan.FromHours(-5),
                ETimeZone.Eastern => TimeSpan.FromHours(-4),
                _ => TimeSpan.Zero
            };

            Assert.Equal(local - expectedOffset, utc.UtcDateTime);
        }
    }

    [Fact]
    public void BackAndForthConversions_ShouldBeIdempotent()
    {
        foreach (var zone in Zones)
        {
            DateTime local = new(2025, 8, 15, 15, 30, 0, DateTimeKind.Unspecified);
            DateTimeOffset utc = _translator.Convert(local, zone);

            // Repeat 100 times: convert back to local (simulate) then to UTC again
            DateTimeOffset current = utc;
            for (int i = 0; i < 100; i++)
            {
                DateTime simulatedLocal = current.UtcDateTime + GetZoneOffset(zone, simulatedLocalMonth: current.UtcDateTime.Month);
                current = _translator.Convert(simulatedLocal, zone);
            }

            DateTimeOffset finalUtc = current;
            Assert.Equal(utc.UtcDateTime, finalUtc.UtcDateTime);
        }
    }

    [Fact]
    public void InvalidTime_ShouldBeAdjustedDeterministically()
    {
        // Example: Pacific DST start 2025 -> 2 AM skips to 3 AM
        DateTime invalid = new(2025, 3, 9, 2, 30, 0, DateTimeKind.Unspecified);
        DateTimeOffset firstConversion = _translator.Convert(invalid, ETimeZone.Pacific);

        DateTimeOffset current = firstConversion;
        for (int i = 0; i < 100; i++)
        {
            // Convert back to local
            DateTime simulatedLocal = current.UtcDateTime + TimeSpan.FromHours(-7); // pre-DST offset
            current = _translator.Convert(simulatedLocal, ETimeZone.Pacific);
        }

        Assert.Equal(firstConversion.UtcDateTime, current.UtcDateTime);
    }

    [Fact]
    public void EdgeCases_ShouldHandleLeapYear_EndOfMonth_Midnight()
    {
        var testDates = new[]
        {
            new DateTime(2024, 2, 29, 0, 0, 0), // Leap year
            new DateTime(2025, 12, 31, 23, 59, 59), // End of year
            new DateTime(2025, 6, 15, 0, 0, 0), // Midnight
            new DateTime(2025, 11, 2, 1, 30, 0) // DST fall-back ambiguous
        };

        foreach (var zone in Zones)
        {
            foreach (var dt in testDates)
            {
                DateTimeOffset first = _translator.Convert(dt, zone);
                DateTimeOffset second = _translator.Convert(dt, zone);

                Assert.Equal(first.UtcDateTime, second.UtcDateTime);
            }
        }
    }

    [Fact]
    public void OutMethod_ShouldReturnTrueAndEqualResult()
    {
        foreach (var zone in Zones)
        {
            DateTime local = new(2025, 5, 10, 14, 0, 0, DateTimeKind.Unspecified);
            bool success = _translator.Convert(local, zone, out DateTimeOffset result);

            Assert.True(success);
            Assert.Equal(_translator.Convert(local, zone), result);
        }
    }

    #region Helpers

    private static TimeSpan GetZoneOffset(ETimeZone zone, int simulatedLocalMonth)
    {
        // Rough DST check for US zones
        bool isDst = simulatedLocalMonth is >= 3 and <= 11; // Approximate
        return zone switch
        {
            ETimeZone.Pacific => TimeSpan.FromHours(isDst ? -7 : -8),
            ETimeZone.Mountain => TimeSpan.FromHours(isDst ? -6 : -7),
            ETimeZone.Central => TimeSpan.FromHours(isDst ? -5 : -6),
            ETimeZone.Eastern => TimeSpan.FromHours(isDst ? -4 : -5),
            _ => TimeSpan.Zero
        };
    }

    #endregion
}
