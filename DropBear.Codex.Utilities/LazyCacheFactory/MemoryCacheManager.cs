using DropBear.Codex.AppLogger.Builders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

/// <summary>
///     Manages caching operations using IMemoryCache.
/// </summary>
public class MemoryCacheManager
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheManager> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MemoryCacheManager" /> class.
    /// </summary>
    /// <param name="cache">The IMemoryCache instance to use for caching.</param>
    public MemoryCacheManager(IMemoryCache cache)
    {
        _cache = cache;

        var loggerFactory = new LoggerConfigurationBuilder()
            .SetLogLevel(LogLevel.Information)
            .EnableConsoleOutput()
            .Build();

        _logger = loggerFactory.CreateLogger<MemoryCacheManager>();
    }

    /// <summary>
    ///     Gets an item from the cache or creates it if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type of the item to get or create.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="createItem">The function to create the item if it doesn't exist in the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration time for the cache entry.</param>
    /// <returns>The cached item or the newly created item.</returns>
    public T GetOrCreate<T>(object key, Func<T> createItem, TimeSpan? absoluteExpiration = null)
    {
        if (key is null)
        {
            _logger.ZLogError($"Cache key cannot be null.");
            throw new ArgumentNullException(nameof(key), "Cache key cannot be null.");
        }

        if (createItem is null)
        {
            _logger.ZLogError($"Creation function cannot be null.");
            throw new ArgumentNullException(nameof(createItem), "Creation function cannot be null.");
        }

        try
        {
            return _cache.GetOrCreate(key, entry =>
            {
                if (absoluteExpiration.HasValue)
                    entry.AbsoluteExpirationRelativeToNow = absoluteExpiration;

                try
                {
                    var item = createItem();
                    if (item is not null) return item;
                    _logger.ZLogWarning($"Created item for key {key} is null.");
                    throw new InvalidOperationException("Created item cannot be null.");

                }
                catch (Exception ex)
                {
                    _logger.ZLogError(ex, $"Error occurred creating item for cache key {key}.");
                    throw;
                }
            }) ?? throw new InvalidOperationException( $"Failed to access cache for key {key}.");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"Failed to access cache for key {key}.");
            throw;
        }
    }

    /// <summary>
    ///     Gets an item from the cache or creates it asynchronously if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type of the item to get or create.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="createItem">The asynchronous function to create the item if it doesn't exist in the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration time for the cache entry.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached item or the newly created item.</returns>
    public async Task<T> GetOrCreateAsync<T>(object key, Func<CancellationToken, Task<T>> createItem,
        TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        if (key is null)
        {
            _logger.ZLogError($"Cache key cannot be null.");
            throw new ArgumentNullException(nameof(key), "Cache key cannot be null.");
        }

        if (createItem is null)
        {
            _logger.ZLogError($"Creation function cannot be null.");
            throw new ArgumentNullException(nameof(createItem), "Creation function cannot be null.");
        }

        try
        {
            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                if (absoluteExpiration.HasValue)
                    entry.AbsoluteExpirationRelativeToNow = absoluteExpiration;

                try
                {
                    var item = await createItem(cancellationToken).ConfigureAwait(false);

                    if (item is not null) return item;
                    _logger.ZLogWarning($"Created item for key {key} is null.");
                    throw new InvalidOperationException("Created item cannot be null.");

                }
                catch (Exception ex)
                {
                    _logger.ZLogError(ex, $"Error occurred while creating item for cache key {key}.");
                    throw;
                }
            }).ConfigureAwait(false) ?? default!;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"Failed to access cache or handle item creation for key {key}.");
            throw;
        }
    }

    /// <summary>
    ///     Removes an item from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    public void Remove(object key)
    {
        if (key is null)
        {
            _logger.ZLogError($"Cache key cannot be null.");
            throw new ArgumentNullException(nameof(key), "Cache key cannot be null.");
        }

        _cache.Remove(key);
    }

    /// <summary>
    ///     Clears all items from the cache.
    /// </summary>
    public void Clear() => _cache.Dispose();
}
