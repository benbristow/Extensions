using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BenBristow.Extensions.UnitTests;

public class DistributedCacheExtensionsTests
{
    private readonly Mock<IDistributedCache> _cacheMock = new();

    [Fact]
    public async Task GetOrCreateAsync_ItemNotInCache_CreatesAndCachesItem()
    {
        // Arrange
        const string key = "testKey";
        var expectedItem = new TestItem { Value = "test" };
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
        _cacheMock.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _cacheMock.Object.GetOrCreateAsync(key, _ => Task.FromResult(expectedItem));

        // Assert
        Assert.Equal(expectedItem.Value, result.Value);
        _cacheMock.Verify(c => c.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_ItemInCache_ReturnsCachedItem()
    {
        // Arrange
        const string key = "testKey";
        var cachedItem = new TestItem { Value = "cached" };
        var cachedBytes = JsonSerializer.SerializeToUtf8Bytes(cachedItem);
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(cachedBytes);

        // Act
        var result = await _cacheMock.Object.GetOrCreateAsync(key, _ => Task.FromResult(new TestItem { Value = "new" }));

        // Assert
        Assert.Equal(cachedItem.Value, result.Value);
        _cacheMock.Verify(c => c.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithExpiry_SetsExpiryOption()
    {
        // Arrange
        const string key = "testKey";
        var expiry = TimeSpan.FromMinutes(5);
        var expectedItem = new TestItem { Value = "test" };
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);
        _cacheMock.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _cacheMock.Object.GetOrCreateAsync(key, _ => Task.FromResult(expectedItem), expiry);

        // Assert
        Assert.Equal(expectedItem.Value, result.Value);
        _cacheMock.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiry), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_WithCancellationToken_PassesToken()
    {
        // Arrange
        const string key = "testKey";
        var cts = new CancellationTokenSource();
        var expectedItem = new TestItem { Value = "test" };
        _cacheMock.Setup(c => c.GetAsync(key, cts.Token)).ReturnsAsync((byte[]?)null);
        _cacheMock.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), cts.Token)).Returns(Task.CompletedTask);

        // Act
        var result = await _cacheMock.Object.GetOrCreateAsync(key, _ => Task.FromResult(expectedItem), cancellationToken: cts.Token);

        // Assert
        Assert.Equal(expectedItem.Value, result.Value);
        _cacheMock.Verify(c => c.GetAsync(key, cts.Token), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_DeserializationFails_ThrowsException()
    {
        // Arrange
        const string key = "testKey";
        var invalidBytes = new byte[] { 1, 2, 3 }; // Invalid JSON
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(invalidBytes);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _cacheMock.Object.GetOrCreateAsync<TestItem>(key, _ => Task.FromResult(new TestItem())));
    }

    private class TestItem
    {
        public string Value { get; init; } = string.Empty;
    }
}
