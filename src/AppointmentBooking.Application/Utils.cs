using System.Text.RegularExpressions;
namespace AppointmentBooking.Application;

public static class Utils
{
    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
    }

    public static string ConvertUtcToTimeZone(TimeSpan utcTime, string timeZoneId)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var referenceDate = DateTime.UtcNow.Date;
            var utcDateTime = referenceDate.Add(utcTime);

            var targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);

            var targetTime = targetDateTime.TimeOfDay;

            var totalSeconds = (int)targetTime.TotalSeconds;
            var normalizedSeconds = totalSeconds % (24 * 60 * 60);
            var normalizedTime = TimeSpan.FromSeconds(normalizedSeconds);

            return normalizedTime.ToString(@"hh\:mm\:ss");
        }
        catch (TimeZoneNotFoundException)
        {
            throw new ArgumentException($"Timezone '{timeZoneId}' not found. Please use a valid timezone identifier.", nameof(timeZoneId));
        }
        catch (InvalidTimeZoneException)
        {
            throw new ArgumentException($"Timezone '{timeZoneId}' is invalid.", nameof(timeZoneId));
        }
    }
}