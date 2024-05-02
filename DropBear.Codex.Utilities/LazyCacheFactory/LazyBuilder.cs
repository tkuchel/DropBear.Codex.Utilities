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

    private Func<Task<T>> _asyncInitializer =
        () => throw new InvalidOperationException("Asynchronous initialization function not set.");

    private TimeSpan? _cacheExpiration;

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
    ///     Specifies the asynchronous initialization function for the lazy instance.
    /// </summary>
    /// <param name="asyncInitializer">A function that returns a Task of <typeparamref name="T" />.</param>
    /// <returns>The current instance of <see cref="LazyBuilder{T}" />.</returns>
    public ILazyConfiguration<T> WithAsyncInitialization(Func<Task<T>> asyncInitializer)
    {
        _asyncInitializer = asyncInitializer ??
                            throw new ArgumentNullException(nameof(asyncInitializer),
                                "Async initializer cannot be null.");
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

    /// <summary>
    ///     Builds a ResettableLazy&lt;T&gt; instance using a potentially asynchronous initialization function,
    ///     forcing it to run synchronously. Use with caution as this can lead to deadlocks and performance issues.
    ///     This variation allows the value to be reset and recomputed.
    /// </summary>
    /// <returns>A ResettableLazy&lt;T&gt; that will initialize the value on first access and can be reset to reinitialize.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initializer is set.</exception>
    public ResettableLazy<T> BuildWithForcedSyncResettable()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new ResettableLazy<T>(() =>
        {
            // Using Task.Run to offload to the thread pool and calling .Result to force synchronization
            return Task.Run(async () => await (_cacheExpiration.HasValue
                ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
                : _asyncInitializer()).ConfigureAwait(false)).Result;
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

        return new ResettableLazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreate(typeof(T).ToString(), _initializer, _cacheExpiration)
            : _initializer());
    }


    /// <summary>
    ///     Builds a resettable lazy instance using the specified asynchronous initialization function.
    ///     This method synchronously wraps the asynchronous operation, which can lead to performance implications.
    ///     Use with caution as this method forces asynchronous operations to complete synchronously, potentially causing
    ///     deadlocks and performance bottlenecks.
    /// </summary>
    /// <returns>
    ///     A <see cref="ResettableLazy{T}" /> that encapsulates a lazily initialized value with the ability to reset. The
    ///     initialization is handled asynchronously but consumed synchronously.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initialization function has been set.</exception>
    public ResettableLazy<T> BuildResettableAsync()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new ResettableLazy<T>(() =>
        {
            var result = Task.Run(async () => await (_cacheExpiration.HasValue
                ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
                : _asyncInitializer()).ConfigureAwait(false)).Result;
            return result;
        });
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

        return new AsyncResettableLazy<T>(() => _cacheExpiration.HasValue
            ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
            : _asyncInitializer());
    }

    /// <summary>
    ///     Builds an AsyncResettableLazy&lt;T&gt; instance using a potentially asynchronous initialization function,
    ///     forcing it to run synchronously. This method allows the value to be reset and recomputed asynchronously,
    ///     but values are retrieved synchronously, which can lead to deadlocks and performance issues. Use with caution.
    /// </summary>
    /// <returns>
    ///     An AsyncResettableLazy&lt;T&gt; that will initialize the value asynchronously on first access and can be
    ///     reset, with forced synchronous retrieval.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if no asynchronous initialization function has been set.</exception>
    public AsyncResettableLazy<T> BuildWithForcedSyncAsyncResettable()
    {
        if (_asyncInitializer is null)
            throw new InvalidOperationException("Asynchronous initializer must be set.");

        return new AsyncResettableLazy<T>(async () =>
        {
            var task = _cacheExpiration.HasValue
                ? _cacheManager.GetOrCreateAsync(typeof(T).ToString(), _asyncInitializer, _cacheExpiration)
                : _asyncInitializer();

            // Force the task to run and block until completion
            var result = await Task.Run(() => task).ConfigureAwait(false);
            return result;
        });
    }
}
