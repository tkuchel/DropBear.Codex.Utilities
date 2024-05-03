using System.Runtime.CompilerServices;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

/// <summary>
/// Provides an asynchronous resettable lazy initialization pattern.
/// </summary>
/// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
public class AsyncResettableLazy<T> : IDisposable
{
    private readonly object _lock = new();
    private readonly Func<CancellationToken, Task<T>> _taskFactory;
    private Task<T> _task;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncResettableLazy{T}"/> class with the specified asynchronous value factory.
    /// </summary>
    /// <param name="taskFactory">The asynchronous factory function that creates the value.</param>
    public AsyncResettableLazy(Func<CancellationToken, Task<T>> taskFactory)
    {
        _taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
        _task = null!;
        _cts = new CancellationTokenSource();
        IsValueCreated = false;
    }

    /// <summary>
    /// Gets a task representing the asynchronous initialization of the value.
    /// </summary>
    public Task<T> Value
    {
        get
        {
            lock (_lock)
            {
                if (IsValueCreated) return _task;
                _task = ExecuteTaskFactory();
                IsValueCreated = true;

                return _task;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the value has been created.
    /// </summary>
    public bool IsValueCreated { get; private set; }

    /// <summary>
    /// Resets the lazy value, allowing it to be recreated on the next access.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            IsValueCreated = false;
            _task = null!;
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
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

    private Task<T> ExecuteTaskFactory()
    {
        try
        {
            return _taskFactory(_cts.Token);
        }
        catch
        {
            IsValueCreated = false; // Allow retry if factory method throws immediately
            throw;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}