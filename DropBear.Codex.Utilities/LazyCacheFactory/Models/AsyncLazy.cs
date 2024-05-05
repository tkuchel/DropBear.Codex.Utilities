using System.Runtime.CompilerServices;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

/// <summary>
/// Provides an asynchronous lazy initialization pattern.
/// </summary>
/// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{T}"/> class with the specified asynchronous value factory.
    /// </summary>
    /// <param name="taskFactory">The asynchronous factory function that creates the value.</param>
    /// <param name="runOnThreadPool">Specifies whether the factory function should be executed on the thread pool.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the asynchronous initialization.</param>
    public AsyncLazy(Func<CancellationToken, Task<T>> taskFactory, bool runOnThreadPool = true, CancellationToken cancellationToken = default) :
        base(() => InitializeAsync(taskFactory, runOnThreadPool, cancellationToken), true) // True to make it thread-safe
    {
    }
    
    private static Task<T> InitializeAsync(Func<CancellationToken, Task<T>> taskFactory, bool runOnThreadPool, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<T>(cancellationToken);

        return runOnThreadPool ? Task.Run(() => taskFactory(cancellationToken), cancellationToken) : taskFactory(cancellationToken);
    }

    /// <summary>
    /// Gets an awaiter used to await the completion of the asynchronous initialization.
    /// </summary>
    /// <returns>An awaiter instance.</returns>
    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();

    /// <summary>
    /// Configures how awaits on the tasks returned from asynchronous initialization should be performed.
    /// </summary>
    /// <param name="continueOnCapturedContext">Specifies whether to capture and marshal back to the original context.</param>
    /// <returns>A configured task.</returns>
    public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext) => Value.ConfigureAwait(continueOnCapturedContext);
}