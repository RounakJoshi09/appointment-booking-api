using System.Text.RegularExpressions;
namespace AppointmentBooking.Application;

public static class Utils
{
    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
    }
}