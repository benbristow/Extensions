using System.ComponentModel;

namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the description of an enum value. If the enum value has a <see cref="DescriptionAttribute"/>,
    /// the description from the attribute is returned. Otherwise, the enum's name is returned.
    /// </summary>
    /// <param name="value">The enum value for which to get the description.</param>
    /// <returns>The description of the enum value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided enum value does not exist within the enum type.</exception>
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null)
            throw new ArgumentOutOfRangeException(nameof(value), "Invalid enum value");

        var descriptionAttribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        return descriptionAttribute?.Description ?? value.ToString();
    }
}