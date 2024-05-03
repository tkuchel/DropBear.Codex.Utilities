using DropBear.Codex.Utilities.LazyCacheFactory.Interfaces;
using DropBear.Codex.Utilities.LazyCacheFactory.Models;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

/// <summary>
///     Provides a builder for creating lazy instances of type <typeparamref name="T" />, supporting both synchronous and
///     asynchronous initialization.
/// </summary>
/// <typeparam name="T">The type of object to be lazily instantiated.</typeparam>
public class LazyBuilder<T> : ILazyConfiguration<T>
{
    private readonly MemoryCacheManager _cacheManager;

    private Func<CancellationToken, Task<T>> _asyncInitializer =
        _ => throw new InvalidOperationException("Asynchronous initialization function not set.");

    private TimeSpan? _cacheExpiration;

    private T _defaultValue = default!;

    private Func<T> _initializer = () =>
        throw new InvalidOperationException("Synchronization initialization function not set.");

    /// <summary>
    ///     Initializes a new instance of the <see cref="LazyBuilder{T}" /> class.
    /// </summary>
    /// <param name="cacheManager">The cache manager used to manage caching for lazy instances.</param>
    public LazyBuilder(MemoryCacheManager cacheManager) => _cacheManager = cacheManager;

    /// <summary>
    ///     Specifies the initialization function for the lazy instance.
    /// </summary>
    /// <param name="initializer">A function that returns an instance of <typeparamref name="T" />.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithInitialization(Func<T> initializer)
    {
        _initializer = initializer ??
                       throw new ArgumentNullException(nameof(initializer), "Initializer cannot be null.");
        return this;
    }

    /// <summary>
    ///     Specifies the cache expiration for the lazy instance.
    /// </summary>
    /// <param name="expiration">The timespan after which the cache should expire.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithCaching(TimeSpan expiration = default)
    {
        _cacheExpiration = expiration == default ? TimeSpan.FromMinutes(5) : expiration;
        return this;
    }

    /// <summary>
    ///     Builds the lazy instance using the specified initialization function.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no initialization function has been set.</exception>
    /// <returns>A lazy wrapper around the initialization function.</returns>
    public Lazy<T> Build()
    {
        if (_initializer is null)
            throw new InvalidOperationException("Synchronous initializer must be set.");

        return new Lazy<T>(() =>
        {
            try
            {
                return _cacheExpiration.HasValue
                    ? _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration.Value)
                    : _initializer();
            }
            catch
            {
                return _defaultValue;
            }
        });
    }

    /// <summary>
    ///     Builds a resettable lazy instance using the specified synchronous initialization function.
    ///     This allows the lazy value to be reset and recomputed when needed.
    /// </summary>
    /// <returns>A <see cref="ResettableLazy{T}" /> that encapsulates a lazily initialized value with the ability to reset.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no synchronous initialization function has been set.</exception>
    public ResettableLazy<T> BuildResettable()
    {
        if (_initializer is null)
            throw new InvalidOperationException("Synchronous initializer must be set.");

        return new ResettableLazy<T>(() =>
        {
            try
            {
                return _cacheExpiration.HasValue
                    ? _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration.Value)
                    : _initializer();
            }
            catch
            {
                return _defaultValue;
            }
        }, _defaultValue);
    }

    /// <summary>
    ///     Builds an AsyncResettableLazy&lt;T&gt; instance using the specified asynchronous initialization function.
    /// </summary>
    /// <returns>An AsyncResettableLazy&lt;T&gt; that will initialize the value asynchronously and can be reset.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initialization function has been set.</exception>
    public AsyncResettableLazy<T> BuildAsyncResettable()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new AsyncResettableLazy<T>(cancellationToken =>
        {
            try
            {
                return _cacheExpiration.HasValue
                    ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration.Value,
                        cancellationToken)
                    : _asyncInitializer(cancellationToken);
            }
            catch
            {
                return Task.FromResult(_defaultValue);
            }
        });
    }

    /// <summary>
    ///     Specifies the asynchronous initialization function for the lazy instance.
    /// </summary>
    /// <param name="asyncInitializer">A function that returns a Task of <typeparamref name="T" />.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithAsyncInitialization(Func<CancellationToken, Task<T>> asyncInitializer)
    {
        _asyncInitializer = asyncInitializer ??
                            throw new ArgumentNullException(nameof(asyncInitializer),
                                "Async initializer cannot be null.");
        return this;
    }

    /// <summary>
    ///     Specifies the default value to be returned if the initialization function throws an exception.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithDefaultValue(T defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    /// <summary>
    ///     Builds the asynchronous lazy instance using the specified asynchronous initialization function.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initialization function has been set.</exception>
    /// <returns>An asynchronous lazy wrapper around the initialization function.</returns>
    public AsyncLazy<T> BuildAsync(CancellationToken cancellationToken = default)
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new AsyncLazy<T>(async ct =>
        {
            try
            {
                return await (_cacheExpiration.HasValue
                    ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration.Value,
                        ct)
                    : _asyncInitializer(ct)).ConfigureAwait(false);
            }
            catch
            {
                return _defaultValue;
            }
        }, true, cancellationToken);
    }
}
