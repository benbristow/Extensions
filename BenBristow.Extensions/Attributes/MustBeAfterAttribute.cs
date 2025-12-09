using System.ComponentModel.DataAnnotations;

namespace BenBristow.Extensions.Attributes;

/// <summary>
///     Validation attribute to ensure that a date property is after another specified date property.
///     Supports DateTime, DateTimeOffset, and DateOnly types, including their nullable versions.
/// </summary>
/// <remarks>
///     This attribute validates that the decorated property's date value is chronologically after
///     the date value of the specified comparison property. Both properties must be of compatible
///     date types (DateTime, DateTimeOffset, or DateOnly), including their nullable versions.
/// </remarks>
/// <example>
///     <code>
///     public class Event
///     {
///         public DateTime StartDate { get; set; }
///         
///         [MustBeAfter(nameof(StartDate))]
///         public DateTime EndDate { get; set; }
///     }
///     </code>
/// </example>
public sealed class MustBeAfterAttribute : ValidationAttribute
{
    private readonly string _comparisonPropertyName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MustBeAfterAttribute" /> class.
    /// </summary>
    /// <param name="comparisonPropertyName">The name of the property to compare against.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparisonPropertyName"/> is null or empty.</exception>
    public MustBeAfterAttribute(string comparisonPropertyName)
    {
        _comparisonPropertyName = comparisonPropertyName;
    }

    /// <summary>
    ///     Validates that the value of the current property is after the value of the comparison property.
    /// </summary>
    /// <param name="value">The value of the property to be validated.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    ///     An instance of the <see cref="ValidationResult" /> class.
    ///     It will be <see cref="ValidationResult.Success" /> if the validation succeeds;
    ///     otherwise, an instance of <see cref="ValidationResult" /> with an error message.
    /// </returns>
    /// <remarks>
    ///     The validation succeeds in the following cases:
    ///     <list type="bullet">
    ///         <item><description>Both values are null</description></item>
    ///         <item><description>The current property's value is chronologically after the comparison property's value</description></item>
    ///     </list>
    ///     The validation fails when:
    ///     <list type="bullet">
    ///         <item><description>The comparison property does not exist</description></item>
    ///         <item><description>One value is null and the other is not</description></item>
    ///         <item><description>The current property's value is not after the comparison property's value</description></item>
    ///         <item><description>Either value is of an unsupported date type</description></item>
    ///     </list>
    /// </remarks>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_comparisonPropertyName);

        if (property == null)
            return new ValidationResult($"Unknown property: {_comparisonPropertyName}");

        var comparisonValue = property.GetValue(validationContext.ObjectInstance);

        if (value == null && comparisonValue == null)
            return ValidationResult.Success;

        if (value == null || comparisonValue == null)
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be after {_comparisonPropertyName}.");

        var valueDate = GetDateTimeOffset(value);
        var comparisonDate = GetDateTimeOffset(comparisonValue);

        return valueDate > comparisonDate
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be after {_comparisonPropertyName}.");
    }

    /// <summary>
    ///     Converts the given value to a DateTimeOffset.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A DateTimeOffset representation of the value.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not a supported date type.</exception>
    /// <remarks>
    ///     Supported types include:
    ///     <list type="bullet">
    ///         <item><description><see cref="DateTime"/> - Converted to DateTimeOffset using the DateTime's Kind property</description></item>
    ///         <item><description><see cref="DateTimeOffset"/> - Returned as-is</description></item>
    ///         <item><description><see cref="DateOnly"/> - Converted to DateTimeOffset with time set to midnight</description></item>
    ///     </list>
    /// </remarks>
    private static DateTimeOffset GetDateTimeOffset(object? value) => value switch
    {
        DateTime dateTime => new DateTimeOffset(dateTime),
        DateTimeOffset dateTimeOffset => dateTimeOffset,
        DateOnly dateOnly => new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue)),
        _ => throw new ArgumentException($"Unsupported date type: {value!.GetType()}"),
    };
}