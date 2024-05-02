using DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

public class LazyBuilder<T> : ILazyConfiguration<T>
{
    private readonly MemoryCacheManager _cacheManager;
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

    public Lazy<T> Build() =>
        _cacheExpiration.HasValue
            ? new Lazy<T>(() => _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration))
            : new Lazy<T>(_initializer);
}
