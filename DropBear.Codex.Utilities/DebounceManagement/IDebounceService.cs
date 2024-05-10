using System.Runtime.CompilerServices;
using DropBear.Codex.Core;

namespace DropBear.Codex.Utilities.DebounceManagement;

public interface IDebounceService
{
    /// <summary>
    ///     Debounces a function that returns a generic Result
    ///     <T>
    ///         . Ensures that the function is not executed more frequently
    ///         than the specified minimum interval.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="function">The function to execute which returns a Task of Result<T>.</param>
    /// <param name="key">
    ///     An optional unique identifier for the function call used for debouncing purposes. If not provided,
    ///     a key will be generated based on the caller's details.
    /// </param>
    /// <param name="debounceTime">
    ///     The minimum time interval between successive executions. If not provided, a default
    ///     interval of 30 seconds is used.
    /// </param>
    /// <param name="caller">Automatically filled by the compiler to provide the method name of the caller.</param>
    /// <param name="filePath">Automatically filled by the compiler to provide the source file path of the caller.</param>
    /// <param name="lineNumber">
    ///     Automatically filled by the compiler to provide the line number in the source code of the
    ///     caller.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing a Result
    ///     <T> indicating the outcome of the function execution.
    /// </returns>
    Task<Result<T>> DebounceAsync<T>(
        Func<Task<Result<T>>> function,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    ///     Debounces an action that does not return a value. Ensures that the action is not executed more frequently
    ///     than the specified minimum interval.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="key">
    ///     An optional unique identifier for the action call used for debouncing purposes. If not provided,
    ///     a key will be generated based on the caller's details.
    /// </param>
    /// <param name="debounceTime">
    ///     The minimum time interval between successive executions. If not provided, a default
    ///     interval of 30 seconds is used.
    /// </param>
    /// <param name="caller">Automatically filled by the compiler to provide the method name of the caller.</param>
    /// <param name="filePath">Automatically filled by the compiler to provide the source file path of the caller.</param>
    /// <param name="lineNumber">
    ///     Automatically filled by the compiler to provide the line number in the source code of the
    ///     caller.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing a Result indicating the outcome of the action
    ///     execution.
    /// </returns>
    Task<Result> DebounceAsync(
        Action action,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    ///     Debounces a function that returns a Task of Result. This method ensures that the function is not executed
    ///     more frequently than the specified minimum interval. If the function is called again before the interval
    ///     has elapsed, it returns a failure Result indicating that the operation was debounced.
    /// </summary>
    /// <param name="function">The asynchronous function to execute, expected to return a Task of Result.</param>
    /// <param name="key">
    ///     An optional unique identifier for the function call used for debouncing purposes.
    ///     If not provided, a key will be generated based on the caller's details.
    /// </param>
    /// <param name="debounceTime">
    ///     The minimum time interval between successive executions. If not provided,
    ///     a default interval of 30 seconds is used.
    /// </param>
    /// <param name="caller">
    ///     Automatically filled by the compiler to provide the method name of the caller. This is
    ///     used to generate a unique key if no explicit key is provided.
    /// </param>
    /// <param name="filePath">
    ///     Automatically filled by the compiler to provide the source file path of the caller. This is
    ///     also used in key generation if no explicit key is provided.
    /// </param>
    /// <param name="lineNumber">
    ///     Automatically filled by the compiler to provide the line number in the source code of the
    ///     caller. This helps further in generating a uniquely identifying key if required.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation, which upon completion contains a Result indicating
    ///     the outcome of the debounced function execution. If the function is debounced, the Result will indicate failure
    ///     with a message stating "Operation debounced." If the function throws an exception, the Result will also indicate
    ///     failure with a message describing the exception.
    /// </returns>
    Task<Result> DebounceAsync(
        Func<Task<Result>> function,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);
}
