namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for DateTimeOffset.
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    /// Converts the specified DateTimeOffset to a locale-specific datetime string.
    /// Uses the "g" format (general date/time pattern, short time) which formats as:
    /// - Short date pattern followed by short time pattern
    /// - Culture-specific formatting (e.g., "4/15/2023 2:30 PM" for en-US, "15.04.2023 14:30" for de-DE)
    /// </summary>
    /// <param name="dateTime">The DateTimeOffset to convert.</param>
    /// <returns>A locale-specific datetime string representation of the DateTimeOffset.</returns>
    /// <example>
    /// <code>
    /// var date = new DateTimeOffset(2023, 4, 15, 14, 30, 0, TimeSpan.Zero);
    /// string formatted = date.ToLocaleDateTimeString();
    /// // Returns "4/15/2023 2:30 PM" in en-US culture
    /// </code>
    /// </example>
    public static string ToLocaleDateTimeString(this DateTimeOffset dateTime) => dateTime.ToString("g");
}