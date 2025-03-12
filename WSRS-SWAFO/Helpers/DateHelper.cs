using System.Globalization;

namespace WSRS_SWAFO.Helpers;

public static class DateHelper
{
    public static DateOnly ParseDate(string dateString)
    {
        if (DateOnly.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new FormatException($"Invalid date format: {dateString}");
    }

    public static DateOnly? ParseDateNullable(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }
        if (DateOnly.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new FormatException($"Invalid date format: {dateString}");
    }
}