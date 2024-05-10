using DropBear.Codex.Core;

namespace DropBear.Codex.Utilities.DebounceManagement;

public interface IDebounceService
{
    /// <summary>
    ///     Debounces a function returning a generic Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="key">A unique key identifying the function call for debouncing purposes.</param>
    /// <param name="debounceTime">The minimum time interval between successive executions.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function execution.</returns>
    Task<Result<T>> DebounceAsync<T>(Func<Task<Result<T>>> function, string key, TimeSpan debounceTime);

    /// <summary>
    ///     Debounces an action returning a non-generic Result.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="key">A unique key identifying the action call for debouncing purposes.</param>
    /// <param name="debounceTime">The minimum time interval between successive executions.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the action execution.</returns>
    Task<Result> DebounceAsync(Action action, string key, TimeSpan debounceTime);
}
