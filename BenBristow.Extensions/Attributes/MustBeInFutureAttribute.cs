using System.ComponentModel.DataAnnotations;

namespace BenBristow.Extensions.Attributes;

/// <summary>
/// Validation attribute that ensures a date value is in the future.
/// </summary>
/// <remarks>
/// This attribute can be applied to properties, fields, or parameters of type <see cref="DateTime"/>, 
/// <see cref="DateTimeOffset"/>, or <see cref="DateOnly"/>. Null values are considered valid.
/// </remarks>
/// <example>
/// <code>
/// public class Event
/// {
///     [MustBeInFuture]
///     public DateTime EventDate { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class MustBeInFutureAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MustBeInFutureAttribute"/> class with a default error message.
    /// </summary>
    public MustBeInFutureAttribute() : base("The {0} must be a date in the future.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MustBeInFutureAttribute"/> class with a specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message to associate with the validation attribute.</param>
    public MustBeInFutureAttribute(string errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// Validates whether the value is a date in the future.
    /// </summary>
    /// <param name="value">The value to validate. Supported types are <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and <see cref="DateOnly"/>.</param>
    /// <returns>True if the value is null or a date in the future; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not of a supported date type.</exception>
    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // null values are considered valid

        var now = DateTime.Now;

        return value switch
        {
            DateTimeOffset dto => dto > DateTimeOffset.Now,
            DateTime dt => dt > now,
            DateOnly dateOnly => dateOnly > DateOnly.FromDateTime(now),
            _ => throw new ArgumentException("Invalid value type", nameof(value)),
        };
    }

    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>
    /// A <see cref="ValidationResult"/> that represents success if the specified value is valid; otherwise, 
    /// a <see cref="ValidationResult"/> containing an error message.
    /// </returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null && IsValid(value))
            return ValidationResult.Success!;

        var memberName = validationContext.MemberName ?? string.Empty;
        var errorMessage = FormatErrorMessage(memberName);

        return new ValidationResult(errorMessage, [memberName]);
    }
}