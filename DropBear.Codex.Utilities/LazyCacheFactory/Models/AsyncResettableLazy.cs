namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

public class AsyncResettableLazy<T>
{
    private readonly object _lock = new();
    private readonly Func<Task<T>> _taskFactory;
    private Task<T> _task;

    public AsyncResettableLazy(Func<Task<T>> taskFactory)
    {
        _taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
        _task = null!;
        IsValueCreated = false;
    }

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

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsValueCreated { get; private set; }

    public void Reset()
    {
        lock (_lock)
        {
            IsValueCreated = false;
            _task = null!;
        }
    }

    private Task<T> ExecuteTaskFactory()
    {
        try
        {
            return _taskFactory();
        }
        catch
        {
            IsValueCreated = false; // Allow retry if factory method throws immediately
            throw;
        }
    }
}
