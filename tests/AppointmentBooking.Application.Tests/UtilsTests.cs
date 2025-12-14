using AppointmentBooking.Application;
using FluentAssertions;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class UtilsTests
{
    const string IndiaStandardTime = "India Standard Time";
    const string Utc = "UTC";

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("invalid-email", false)]
    [InlineData("missing@domain", false)]
    [InlineData("@domain.com", false)]
    [InlineData("user@", false)]
    [InlineData("", false)]
    public void IsValidEmail_Should_Return_Correct_Result(string email, bool expected)
    {
        var result = Utils.IsValidEmail(email);

        result.Should().Be(expected);
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Convert_Time_Correctly()
    {
        var utcTime = new TimeSpan(10, 30, 0);
        var timeZoneId = Utc;

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{2}:\d{2}:\d{2}$");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Convert_UTC_To_IST_With_5_30_Hours_Difference()
    {
        var utcTime = new TimeSpan(10, 30, 0);
        var timeZoneId = IndiaStandardTime;

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        result.Should().Be("16:00:00");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Throw_When_TimeZone_Not_Found()
    {
        var utcTime = new TimeSpan(10, 30, 0);
        var invalidTimeZoneId = "Invalid/TimeZone";

        var exception = Assert.Throws<ArgumentException>(() =>
            Utils.ConvertUtcToTimeZone(utcTime, invalidTimeZoneId));
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Handle_Time_Wrapping_Over_Midnight()
    {
        var utcTime = new TimeSpan(23, 0, 0);
        var timeZoneId = IndiaStandardTime;

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        result.Should().Be("04:30:00");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Handle_Midnight_UTC()
    {
        var utcTime = new TimeSpan(0, 0, 0);
        var timeZoneId = IndiaStandardTime;

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        result.Should().Be("05:30:00");
    }
    [Fact]
    public void ConvertTimeZoneToUtc_Should_Convert_IST_To_UTC_Correctly()
    {
        var istDateTime = new DateTime(2025, 12, 15, 14, 30, 0);

        var utcDateTime = Utils.ConvertTimeZoneToUtc(istDateTime, IndiaStandardTime);

        utcDateTime.Year.Should().Be(2025);
        utcDateTime.Month.Should().Be(12);
        utcDateTime.Day.Should().Be(15);
        utcDateTime.Hour.Should().Be(9);
        utcDateTime.Minute.Should().Be(0);
        utcDateTime.Kind.Should().Be(DateTimeKind.Utc);
    }


    [Fact]
    public void ConvertTimeZoneToUtc_Should_Handle_Date_Boundary_Crossing_Backward()
    {
        var istDateTime = new DateTime(2025, 12, 15, 2, 0, 0);

        var utcDateTime = Utils.ConvertTimeZoneToUtc(istDateTime, IndiaStandardTime);

        utcDateTime.Year.Should().Be(2025);
        utcDateTime.Month.Should().Be(12);
        utcDateTime.Day.Should().Be(14);
        utcDateTime.Hour.Should().Be(20);
        utcDateTime.Minute.Should().Be(30);
    }

    [Theory]
    [InlineData(2025, 1, 1, 10, 30, 0, 5, 0, 0)] // Jan 1
    [InlineData(2025, 6, 15, 14, 0, 0, 8, 30, 0)] // Mid year
    [InlineData(2025, 12, 31, 23, 59, 59, 18, 29, 59)] // Last moment of year
    public void ConvertTimeZoneToUtc_Should_Handle_Various_Dates(
        int year, int month, int day, int hour, int minute, int second,
        int expectedHour, int expectedMinute, int expectedSecond)
    {
        var istDateTime = new DateTime(year, month, day, hour, minute, second);

        var utcDateTime = Utils.ConvertTimeZoneToUtc(istDateTime, IndiaStandardTime);

        utcDateTime.Hour.Should().Be(expectedHour);
        utcDateTime.Minute.Should().Be(expectedMinute);
        utcDateTime.Second.Should().Be(expectedSecond);
    }

}
