using System.Security.Cryptography;
using System.Text;

namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for string manipulation and cryptographic operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts the first character of a string to uppercase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>A string with the first character in uppercase and the rest of the string unchanged.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is an empty string.</exception>
    /// <example>
    /// <code>
    /// string result = "hello".UppercaseFirstLetter();
    /// // result: "Hello"
    /// </code>
    /// </example>
    public static string UppercaseFirstLetter(this string value) => string.Concat(value[..1].ToUpper(), value.AsSpan(1));

    /// <summary>
    /// Computes the SHA-512 hash for the input string.
    /// </summary>
    /// <param name="value">The input string to hash.</param>
    /// <returns>A hexadecimal string representation of the SHA-512 hash.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <remarks>
    /// This method uses UTF-8 encoding to convert the string to bytes before hashing.
    /// The resulting hash is a 128-character hexadecimal string (512 bits).
    /// </remarks>
    /// <example>
    /// <code>
    /// string hash = "test".HashSHA512();
    /// // hash: "ee26b0dd4af7e749aa1a8ee3c10ae9923f618980772e473f8819a5d4940e0db27ac185f8a0e1d5f84f88bc887fd67b143732c304cc5fa9ad8e6f57f50028a8ff"
    /// </code>
    /// </example>
    public static string HashSha512(this string value)
    {
        var message = Encoding.UTF8.GetBytes(value);
        var hashValue = SHA512.HashData(message);
        return hashValue.Aggregate("", (current, x) => $"{current}{x:x2}");
    }
}