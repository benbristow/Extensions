namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}"/> collections.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Filters a sequence of values based on a predicate if a given condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="condition">A boolean condition that determines whether the predicate should be applied.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition if <paramref name="condition"/> is true; otherwise, the original sequence.</returns>
    /// <example>
    /// <code>
    /// var numbers = new[] { 1, 2, 3, 4, 5 };
    /// var evenNumbers = numbers.WhereIf(true, x => x % 2 == 0);
    /// // Result: { 2, 4 }
    /// 
    /// var allNumbers = numbers.WhereIf(false, x => x % 2 == 0);
    /// // Result: { 1, 2, 3, 4, 5 }
    /// </code>
    /// </example>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate) =>
        condition ? source.Where(predicate) : source;
}