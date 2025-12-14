using AppointmentBooking.Application;
using FluentAssertions;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class UtilsTests
{
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
        // Arrange
        var utcTime = new TimeSpan(10, 30, 0);
        var timeZoneId = "Eastern Standard Time";

        // Act
        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{2}:\d{2}:\d{2}$");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Convert_UTC_To_IST_With_5_30_Hours_Difference()
    {
        var utcTime = new TimeSpan(10, 30, 0);
        var timeZoneId = "India Standard Time";

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        // 10:30:00 UTC + 5:30 hours = 16:00:00 IST
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
        var timeZoneId = "India Standard Time";

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        // 23:00:00 UTC + 5:30:00 = 28:30:00 -> 04:30:00
        result.Should().Be("04:30:00");
    }

    [Fact]
    public void ConvertUtcToTimeZone_Should_Handle_Midnight_UTC()
    {
        var utcTime = new TimeSpan(0, 0, 0);
        var timeZoneId = "India Standard Time";

        var result = Utils.ConvertUtcToTimeZone(utcTime, timeZoneId);

        result.Should().Be("05:30:00");
    }
}
