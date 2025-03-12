using System.Globalization;

namespace WSRS_SWAFO.Helpers;

/// <summary>
/// A helper class for date manipulation
/// </summary>
public static class DateHelper
{
    /// <summary>
    /// Parses date strings to workable DateOnly
    /// </summary>
    /// <param name="dateString">String to parse</param>
    /// <returns>Date only</returns>
    public static DateOnly ParseDate(string dateString)
    {
        if (DateOnly.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new FormatException($"Invalid date format: {dateString}");
    }

    /// <summary>
    /// Parses date strings to workable DateOnly
    /// </summary>
    /// <param name="dateString">String to parse</param>
    /// <returns>Date only</returns>
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