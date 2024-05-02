using System.Runtime.CompilerServices;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Run(taskFactory))
    {
    }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}
