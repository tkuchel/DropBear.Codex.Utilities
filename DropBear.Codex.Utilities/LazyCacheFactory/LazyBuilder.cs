using DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;
using DropBear.Codex.Utilities.LazyCacheFactory.Models;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

public class LazyBuilder<T> : ILazyConfiguration<T>
{
    private readonly MemoryCacheManager _cacheManager;

    private Func<Task<T>> _asyncInitializer =
        () => throw new InvalidOperationException("Initialization function not set.");

    private TimeSpan? _cacheExpiration;
    private Func<T> _initializer = () => throw new InvalidOperationException("Initialization function not set.");

    public LazyBuilder(MemoryCacheManager cacheManager) => _cacheManager = cacheManager;

    public ILazyConfiguration<T> WithInitialization(Func<T> initializer)
    {
        _initializer = initializer;
        return this;
    }

    public ILazyConfiguration<T> WithCaching(TimeSpan expiration)
    {
        _cacheExpiration = expiration;
        return this;
    }

    public Lazy<T> Build()
    {
        if (_asyncInitializer is not null)
            throw new InvalidOperationException("Build method does not support asynchronous initializers.");
        return new Lazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration)
            : _initializer());
    }

    public ILazyConfiguration<T> WithAsyncInitialization(Func<Task<T>> asyncInitializer)
    {
        _asyncInitializer = asyncInitializer;
        return this;
    }

    public AsyncLazy<T> BuildAsync()
    {
        if (_initializer is not null)
            throw new InvalidOperationException("BuildAsync method does not support synchronous initializers.");
        return new AsyncLazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
            : _asyncInitializer());
    }
}
