using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BenBristow.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IDistributedCache"/>.
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Asynchronously retrieves an item from the distributed cache if it exists; otherwise, creates and caches a new item.
    /// </summary>
    /// <typeparam name="T">The type of the item to retrieve or create.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="createItem">An asynchronous function to create the item if it is not in the cache, accepting a cancellation token.</param>
    /// <param name="expiry">Optional absolute expiration time relative to now.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the cached or newly created item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when deserialization of the cached item fails.</exception>
    public static async Task<T> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<CancellationToken, Task<T>> createItem, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);
        T item;
        if (bytes == null)
        {
            item = await createItem(cancellationToken);
            bytes = JsonSerializer.SerializeToUtf8Bytes(item);
            var options = expiry.HasValue ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry } : new DistributedCacheEntryOptions();
            await cache.SetAsync(key, bytes, options, cancellationToken);
        }
        else
        {
            item = JsonSerializer.Deserialize<T>(bytes) ?? throw new InvalidOperationException($"Failed to deserialize cached item for key '{key}'.");
        }
        return item;
    }
}