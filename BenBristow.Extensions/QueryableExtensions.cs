using System.Linq.Expressions;
using BenBristow.Extensions.Enums;

namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to enhance querying capabilities.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Conditionally applies a predicate to filter a sequence of values based on a specified condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">The sequence to apply the predicate to.</param>
    /// <param name="condition">A boolean value that determines whether the predicate should be applied.</param>
    /// <param name="predicate">A function to test each element for a condition. Only invoked if <paramref name="condition"/> is true.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition if <paramref name="condition"/> is true; otherwise, returns the original sequence.</returns>
    /// <example>
    /// <code>
    /// var query = dbContext.Users.WhereIf(includeInactive, u => u.IsActive == false);
    /// </code>
    /// </example>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate) =>
        condition ? source.Where(predicate) : source;

    /// <summary>
    /// Conditionally applies a transformation function to a <see cref="IQueryable{T}"/> based on a provided condition.
    /// If the condition is true, the transformation function is applied to the source queryable. Otherwise, the original source queryable is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source <see cref="IQueryable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IQueryable{T}"/> to potentially transform.</param>
    /// <param name="condition">A boolean value that determines whether the transformation function should be applied.</param>
    /// <param name="func">The transformation function to apply to the source <see cref="IQueryable{T}"/> if the condition is true.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that is either the original source or the result of applying the transformation function, based on the condition.</returns>
    /// <example>
    /// <code>
    /// var query = dbContext.Products
    ///     .ApplyIf(applyPaging, q => q.Skip(10).Take(20))
    ///     .ApplyIf(applyOrdering, q => q.OrderBy(p => p.Name));
    /// </code>
    /// </example>
    public static IQueryable<T> ApplyIf<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> func) =>
        condition ? func(source) : source;

    /// <summary>
    /// Provides a flexible way to order an <see cref="IQueryable{T}"/> based on an enum sort field, sort direction, a configuration dictionary, and a default sort expression.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used to specify the sort field.</typeparam>
    /// <typeparam name="T">The type of elements in the source <see cref="IQueryable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IQueryable{T}"/> to be ordered.</param>
    /// <param name="sortField">The enum value specifying which field to sort by, or null for default sorting.</param>
    /// <param name="sortDirection">The direction to sort (Ascending or Descending). If null, defaults to Ascending.</param>
    /// <param name="orderConfig">A dictionary containing preconfigured ordering strategies for specific enum values and sort directions.</param>
    /// <param name="defaultSort">An expression defining the default sort to be applied when sortField is null.</param>
    /// <returns>An <see cref="IOrderedQueryable{T}"/> of T, sorted according to the specified field, direction, configuration, or default sort.</returns>
    /// <remarks>
    /// <para>If a matching configuration is found in orderConfig for the given sortField and sortDirection, it will be used.</para>
    /// <para>If sortField is null, the default sort will be applied using the defaultSort expression.</para>
    /// <para>If sortDirection is null, it will default to Ascending.</para>
    /// <para>The default sort respects the specified or default sort direction.</para>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the sort field/direction combination is not configured in orderConfig.</exception>
    /// <example>
    /// <code>
    /// var orderConfig = new Dictionary&lt;(UserSortField, SortDirection), Func&lt;IQueryable&lt;User&gt;, IOrderedQueryable&lt;User&gt;&gt;&gt;
    /// {
    ///     [(UserSortField.Name, SortDirection.Ascending)] = q => q.OrderBy(u => u.Name),
    ///     [(UserSortField.Name, SortDirection.Descending)] = q => q.OrderByDescending(u => u.Name),
    ///     [(UserSortField.CreatedDate, SortDirection.Ascending)] = q => q.OrderBy(u => u.CreatedDate),
    ///     [(UserSortField.CreatedDate, SortDirection.Descending)] = q => q.OrderByDescending(u => u.CreatedDate)
    /// };
    /// 
    /// var sortedQuery = dbContext.Users.OrderBy(
    ///     sortField: UserSortField.Name,
    ///     sortDirection: SortDirection.Ascending,
    ///     orderConfig: orderConfig,
    ///     defaultSort: u => u.Id
    /// );
    /// </code>
    /// </example>
    public static IOrderedQueryable<T> OrderBy<TEnum, T>(
        this IQueryable<T> source,
        TEnum? sortField,
        SortDirection? sortDirection,
        Dictionary<(TEnum, SortDirection), Func<IQueryable<T>, IOrderedQueryable<T>>> orderConfig,
        Expression<Func<T, object>> defaultSort)
        where TEnum : struct, Enum
        where T : class
    {
        if (!sortField.HasValue)
            return sortDirection == SortDirection.Descending
                ? source.OrderByDescending(defaultSort)
                : source.OrderBy(defaultSort);

        var direction = sortDirection ?? SortDirection.Ascending;
        if (orderConfig.TryGetValue((sortField.Value, direction), out var configuredOrder))
            return configuredOrder(source);

        throw new ArgumentException($"Sort configuration not found for field {sortField.Value} and direction {direction}");
    }
}