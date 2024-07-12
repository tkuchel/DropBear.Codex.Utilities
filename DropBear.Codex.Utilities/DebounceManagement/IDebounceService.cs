using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DropBear.Codex.Core;

namespace DropBear.Codex.Utilities.DebounceManagement
{
    /// <summary>
    /// Interface for a debounce service to manage debounced function or action calls.
    /// </summary>
    public interface IDebounceService
    {
        /// <summary>
        /// Debounces a function that returns a generic Result&lt;T&gt;.
        /// Ensures that the function is not executed more frequently than the specified minimum interval.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the function.</typeparam>
        /// <param name="function">The function to execute which returns a Task of Result&lt;T&gt;.</param>
        /// <param name="key">An optional unique identifier for the function call used for debouncing purposes.</param>
        /// <param name="debounceTime">The minimum time interval between successive executions.</param>
        /// <param name="caller">Automatically filled by the compiler to provide the method name of the caller.</param>
        /// <param name="filePath">Automatically filled by the compiler to provide the source file path of the caller.</param>
        /// <param name="lineNumber">Automatically filled by the compiler to provide the line number in the source code of the caller.</param>
        /// <returns>A task that represents the asynchronous operation, containing a Result&lt;T&gt; indicating the outcome of the function execution.</returns>
        Task<Result<T>> DebounceAsync<T>(
            Func<Task<Result<T>>> function,
            string key = "",
            TimeSpan? debounceTime = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);

        /// <summary>
        /// Debounces an action that does not return a value.
        /// Ensures that the action is not executed more frequently than the specified minimum interval.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="key">An optional unique identifier for the action call used for debouncing purposes.</param>
        /// <param name="debounceTime">The minimum time interval between successive executions.</param>
        /// <param name="caller">Automatically filled by the compiler to provide the method name of the caller.</param>
        /// <param name="filePath">Automatically filled by the compiler to provide the source file path of the caller.</param>
        /// <param name="lineNumber">Automatically filled by the compiler to provide the line number in the source code of the caller.</param>
        /// <returns>A task that represents the asynchronous operation, containing a Result indicating the outcome of the action execution.</returns>
        Task<Result> DebounceAsync(
            Action action,
            string key = "",
            TimeSpan? debounceTime = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);

        /// <summary>
        /// Debounces a function that returns a Task of Result.
        /// Ensures that the function is not executed more frequently than the specified minimum interval.
        /// </summary>
        /// <param name="function">The asynchronous function to execute.</param>
        /// <param name="key">An optional unique identifier for the function call used for debouncing purposes.</param>
        /// <param name="debounceTime">The minimum time interval between successive executions.</param>
        /// <param name="caller">Automatically filled by the compiler to provide the method name of the caller.</param>
        /// <param name="filePath">Automatically filled by the compiler to provide the source file path of the caller.</param>
        /// <param name="lineNumber">Automatically filled by the compiler to provide the line number in the source code of the caller.</param>
        /// <returns>A task that represents the asynchronous operation, containing a Result indicating the outcome of the debounced function execution.</returns>
        Task<Result> DebounceAsync(
            Func<Task<Result>> function,
            string key = "",
            TimeSpan? debounceTime = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
    }
}
