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
    private TimeSpan? _cacheExpiration;
    private Func<Task<T>> _asyncInitializer = () => throw new InvalidOperationException("Asynchronous initialization function not set.");
    private Func<T> _initializer = () => throw new InvalidOperationException("Synchronization initialization function not set.");


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
        _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer), "Initializer cannot be null.");
        return this;
    }

    /// <summary>
    ///     Specifies the asynchronous initialization function for the lazy instance.
    /// </summary>
    /// <param name="asyncInitializer">A function that returns a Task of <typeparamref name="T" />.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithAsyncInitialization(Func<Task<T>> asyncInitializer)
    {
        _asyncInitializer = asyncInitializer ?? throw new ArgumentNullException(nameof(asyncInitializer), "Async initializer cannot be null.");
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

        return new Lazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration)
            : _initializer());
    }

    /// <summary>
    ///     Builds the asynchronous lazy instance using the specified asynchronous initialization function.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initialization function has been set.</exception>
    /// <returns>An asynchronous lazy wrapper around the initialization function.</returns>
    public AsyncLazy<T> BuildAsync()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new AsyncLazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
            : _asyncInitializer());
    }

    /// <summary>
    ///     Builds a Lazy&lt;T&gt; instance using a potentially asynchronous initialization function,
    ///     forcing it to run synchronously. Use with caution as this can lead to deadlocks and performance issues.
    /// </summary>
    /// <returns>A Lazy&lt;T&gt; that will initialize the value on first access.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initializer is set.</exception>
    public Lazy<T> BuildWithForcedSync()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new Lazy<T>(() =>
        {
            // Using Task.Run to offload to the thread pool and calling .Result to force synchronization
            return Task.Run(async () => await (_cacheExpiration.HasValue
                ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
                : _asyncInitializer()).ConfigureAwait(false)).Result;
        });
    }
}
