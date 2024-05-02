using System.Runtime.CompilerServices;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<Task<T>> taskFactory, bool runOnThreadPool = true) :
        base(() => runOnThreadPool ? Task.Run(taskFactory) : taskFactory())
    {
    }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}
