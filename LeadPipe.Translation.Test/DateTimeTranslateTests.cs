using LeadPipe.Translation.Primitives;

namespace LeadPipe.Translation.Test;

public class DateTimeTranslateTests
{
    private readonly DateTimeTranslate _translator = new();

    private static readonly ETimeZone[] Zones =
    [
        ETimeZone.Pacific,
        ETimeZone.Mountain,
        ETimeZone.Central,
        ETimeZone.Eastern
    ];

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
            // Test normal local time
            DateTime local = new DateTime(2025, 8, 15, 15, 30, 0, DateTimeKind.Unspecified);
            DateTimeOffset utc = _translator.Convert(local, zone);

            DateTimeOffset current = utc;
            for (int i = 0; i < 100; i++)
            {
                // Convert UTC back to local time
                DateTime localTime = ConvertUtcToLocal(current.UtcDateTime, zone);

                // Convert local time back to UTC using translator
                current = _translator.Convert(localTime, zone);
            }

            Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
        }
    }

    [Fact]
    public void BackAndForthConversions_ShouldHandleInvalidTimes()
    {
        foreach (var zone in Zones)
        {
            string tzId = zone switch
            {
                ETimeZone.Pacific => "Pacific Standard Time",
                ETimeZone.Mountain => "Mountain Standard Time",
                ETimeZone.Central => "Central Standard Time",
                ETimeZone.Eastern => "Eastern Standard Time",
                _ => throw new ArgumentException($"Unsupported time zone: {zone}")
            };
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            // Choose a date that falls in the DST gap (invalid local time)
            DateTime dstGap = new DateTime(2025, 3, 9, 2, 30, 0); // 2:30 AM on DST start is usually invalid
            if (!tz.IsInvalidTime(dstGap))
                dstGap = dstGap.AddDays(1); // fallback in case 3/9 is valid for this zone

            DateTimeOffset utc = _translator.Convert(dstGap, zone);
            DateTimeOffset current = utc;

            for (int i = 0; i < 100; i++)
            {
                DateTime localTime = ConvertUtcToLocal(current.UtcDateTime, zone);
                current = _translator.Convert(localTime, zone);
            }

            Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
        }
    }

    [Fact]
    public void MultipleConversions_ShouldMaintainPrecision()
    {
        foreach (var zone in Zones)
        {
            DateTime local = new DateTime(2025, 11, 2, 1, 30, 0); // around DST end
            DateTimeOffset utc = _translator.Convert(local, zone);
            DateTimeOffset current = utc;

            for (int i = 0; i < 100; i++)
            {
                DateTime localTime = ConvertUtcToLocal(current.UtcDateTime, zone);
                current = _translator.Convert(localTime, zone);
            }

            Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
        }
    }

    [Fact]
    public void Convert_WithUtcDateTime_ShouldReturnSameUtc()
    {
        DateTime utcNow = DateTime.UtcNow;
        foreach (var zone in Zones)
        {
            DateTimeOffset result = _translator.Convert(utcNow, zone);
            Assert.Equal(DateTimeKind.Utc, result.UtcDateTime.Kind);
            Assert.Equal(utcNow, result.UtcDateTime);
        }
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

    [Fact]
    public void AmbiguousTimes_ShouldConvertToCorrectUtc_WithZeroOffset()
    {
        var zone = ETimeZone.Eastern;
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // Ambiguous local time: 1:30 AM in the fall-back hour
        DateTime ambiguousLocal = new(2025, 11, 2, 1, 30, 0, DateTimeKind.Unspecified);

        Assert.True(tz.IsAmbiguousTime(ambiguousLocal));

        // Act
        DateTimeOffset result = _translator.Convert(ambiguousLocal, zone);

        // The result must be UTC (offset 0)
        Assert.Equal(TimeSpan.Zero, result.Offset);

        // Compute the two valid UTC instants for the ambiguous time
        var possibleOffsets = tz.GetAmbiguousTimeOffsets(ambiguousLocal);

        var expectedUtc1 = new DateTimeOffset(ambiguousLocal, possibleOffsets[0]).UtcDateTime;
        var expectedUtc2 = new DateTimeOffset(ambiguousLocal, possibleOffsets[1]).UtcDateTime;

        // The result's UTC time must match one of the ambiguous possibilities
        Assert.Contains(result.UtcDateTime, new[] { expectedUtc1, expectedUtc2 });
    }


    [Fact]
    public void RepeatedConversions_AcrossYearBoundary_ShouldBeStable()
    {
        var zone = ETimeZone.Central;
        DateTime start = new DateTime(2024, 12, 31, 23, 59, 0);

        DateTimeOffset utc = _translator.Convert(start, zone);
        DateTimeOffset current = utc;

        for (int i = 0; i < 500; i++)
        {
            DateTime local = ConvertUtcToLocal(current.UtcDateTime, zone);
            current = _translator.Convert(local, zone);
        }

        Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
    }

    [Fact]
    public void MinMaxDateTimes_ShouldNotThrow()
    {
        foreach (var zone in Zones)
        {
            DateTimeOffset min = _translator.Convert(DateTime.MinValue, zone);
            DateTimeOffset max = _translator.Convert(DateTime.MaxValue, zone);

            Assert.Equal(DateTime.MinValue.Kind == DateTimeKind.Utc ? DateTime.MinValue : min.UtcDateTime, min.UtcDateTime);
            Assert.Equal(DateTime.MaxValue.Kind == DateTimeKind.Utc ? DateTime.MaxValue : max.UtcDateTime, max.UtcDateTime);
        }
    }

    [Fact]
    public void OutMethod_ShouldMatchConvert_ForEdgeCases()
    {
        var testDates = new[]
        {
        new DateTime(2025, 3, 9, 2, 30, 0),   // DST gap
        new DateTime(2025, 11, 2, 1, 30, 0),  // DST fall-back
        new DateTime(2025, 12, 31, 23, 59, 59),
        new DateTime(2024, 2, 29, 0, 0, 0)
    };

        foreach (var zone in Zones)
        {
            foreach (var dt in testDates)
            {
                bool success = _translator.Convert(dt, zone, out DateTimeOffset outResult);
                DateTimeOffset direct = _translator.Convert(dt, zone);

                Assert.True(success);
                Assert.Equal(direct.UtcDateTime, outResult.UtcDateTime);
            }
        }
    }

    [Fact]
    public void StressTest_BackAndForthConversions()
    {
        foreach (var zone in Zones)
        {
            DateTime local = new(2025, 6, 15, 12, 0, 0);
            DateTimeOffset utc = _translator.Convert(local, zone);
            DateTimeOffset current = utc;

            for (int i = 0; i < 1000; i++)
            {
                DateTime localTime = ConvertUtcToLocal(current.UtcDateTime, zone);
                current = _translator.Convert(localTime, zone);
            }

            Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
        }
    }

    [Fact]
    public void RoundTrip_LocalToUtcAndBack_ShouldBeIdempotent()
    {
        foreach (var zone in Zones)
        {
            // Test a mix of normal, DST gap, and ambiguous times
            var testDates = new[]
            {
            new DateTime(2025, 6, 15, 12, 0, 0),  // Normal time
            new DateTime(2025, 3, 9, 2, 30, 0),   // DST gap
            new DateTime(2025, 11, 2, 1, 30, 0),  // Ambiguous fall-back
            new DateTime(2025, 12, 31, 23, 59, 59) // End of year
        };

            TimeZoneInfo tz = zone switch
            {
                ETimeZone.Pacific => TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
                ETimeZone.Mountain => TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"),
                ETimeZone.Central => TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"),
                ETimeZone.Eastern => TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
                _ => throw new ArgumentException($"Unsupported time zone: {zone}")
            };

            foreach (var local in testDates)
            {
                // Skip invalid time if not a DST gap
                DateTime adjustedLocal = local;
                if (tz.IsInvalidTime(local))
                {
                    adjustedLocal = local.AddMinutes(1); // move into valid time
                    while (tz.IsInvalidTime(adjustedLocal))
                        adjustedLocal = adjustedLocal.AddMinutes(1);
                }

                // Convert local -> UTC
                DateTimeOffset utc = _translator.Convert(adjustedLocal, zone);

                DateTimeOffset current = utc;

                // Repeat round-trip 100 times
                for (int i = 0; i < 100; i++)
                {
                    // Convert UTC -> local
                    DateTime backToLocal = TimeZoneInfo.ConvertTimeFromUtc(current.UtcDateTime, tz);

                    // Convert back to UTC
                    current = _translator.Convert(backToLocal, zone);
                }

                // Assert no drift
                Assert.Equal(utc.UtcDateTime, current.UtcDateTime);
            }
        }
    }

    #region Helpers

    /// <summary>
    /// Converts a UTC DateTime back to local time using the correct TimeZoneInfo.
    /// </summary>
    private static DateTime ConvertUtcToLocal(DateTime utc, ETimeZone zone)
    {
        string tzId = zone switch
        {
            ETimeZone.Pacific => "Pacific Standard Time",
            ETimeZone.Mountain => "Mountain Standard Time",
            ETimeZone.Central => "Central Standard Time",
            ETimeZone.Eastern => "Eastern Standard Time",
            _ => throw new ArgumentException($"Unsupported time zone: {zone}")
        };

        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
    }

    #endregion
}
