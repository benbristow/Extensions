using System.Collections.Concurrent;

namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ConcurrentBag{T}"/>.
/// </summary>
public static class ConcurrentBagExtensions
{
    /// <summary>
    /// Adds a range of elements to the <see cref="ConcurrentBag{T}"/> in parallel.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="concurrentBag">The <see cref="ConcurrentBag{T}"/> to add items to.</param>
    /// <param name="toAdd">The elements to add to the <see cref="ConcurrentBag{T}"/>.</param>
    /// <remarks>
    /// This method uses <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/> to add elements concurrently,
    /// which may improve performance when adding a large number of items. However, the order of elements in the bag is not guaranteed.
    /// </remarks>
    /// <example>
    /// <code>
    /// var bag = new ConcurrentBag&lt;int&gt;();
    /// bag.AddRange(new[] { 1, 2, 3, 4, 5 });
    /// </code>
    /// </example>
    public static void AddRange<T>(this ConcurrentBag<T> concurrentBag, IEnumerable<T> toAdd) =>
        Parallel.ForEach(toAdd, concurrentBag.Add);
}