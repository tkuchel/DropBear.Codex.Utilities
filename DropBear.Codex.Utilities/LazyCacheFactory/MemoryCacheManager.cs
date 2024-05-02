using DropBear.Codex.AppLogger.Builders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

public class MemoryCacheManager
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheManager> _logger;

    public MemoryCacheManager(IMemoryCache cache)
    {
        _cache = cache;

        var loggerFactory = new LoggerConfigurationBuilder()
            .SetLogLevel(LogLevel.Information)
            .EnableConsoleOutput()
            .Build();

        _logger = loggerFactory.CreateLogger<MemoryCacheManager>();
    }

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
                    if (item is null) _logger.ZLogWarning($"Created item for key {key} is null.");
                    return item;
                }
                catch (Exception ex)
                {
                    _logger.ZLogError(ex, $"Error occurred creating item for cache key {key}.");
                    throw; // Rethrow to maintain exception propagation
                }
            }) ?? throw new InvalidOperationException("Cache item is null.");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"Failed to access cache for key {key}.");
            throw; // It's often useful to let the caller know there was a problem with caching
        }
    }
}
