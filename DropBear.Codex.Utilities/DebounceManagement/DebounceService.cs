using System.Runtime.CompilerServices;
using DropBear.Codex.Core;
using Microsoft.Extensions.Caching.Memory;

namespace DropBear.Codex.Utilities.DebounceManagement;

public class DebounceService : IDebounceService
{
    private readonly TimeSpan _defaultDebounceTime = TimeSpan.FromSeconds(30);
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DebounceService" /> class.
    ///     Uses an IMemoryCache to store timestamps for debouncing function or action calls.
    /// </summary>
    /// <param name="memoryCache">The memory cache used to store timestamps for debouncing.</param>
    public DebounceService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

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
    public async Task<Result<T>> DebounceAsync<T>(
        Func<Task<Result<T>>> function,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        key = GenerateKey(key, caller, filePath, lineNumber);
        debounceTime ??= _defaultDebounceTime;

        if (IsDebounced(key, debounceTime.Value, out var isFirstCall) && !isFirstCall)
            return Result<T>.Failure("Operation debounced.");

        try
        {
            return await function().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure("An error occurred while executing the function.", ex);
        }
    }

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
    public async Task<Result> DebounceAsync(
        Action action,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        key = GenerateKey(key, caller, filePath, lineNumber);
        debounceTime ??= _defaultDebounceTime;

        if (IsDebounced(key, debounceTime.Value, out var isFirstCall) && !isFirstCall)
            return Result.Failure("Operation debounced.");

        try
        {
            await Task.Run(action).ConfigureAwait(false);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("An error occurred while executing the action.", ex);
        }
    }

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
    public async Task<Result> DebounceAsync(
        Func<Task<Result>> function,
        string key = "",
        TimeSpan? debounceTime = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        key = GenerateKey(key, caller, filePath, lineNumber);
        debounceTime ??= _defaultDebounceTime;

        if (IsDebounced(key, debounceTime.Value, out var isFirstCall) && !isFirstCall)
            return Result.Failure("Operation debounced.");

        try
        {
            return await function().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result.Failure("An error occurred while executing the function.", ex);
        }
    }


    /// <summary>
    ///     Checks if a given key has been debounced within the specified timeframe and identifies if it's the first call.
    /// </summary>
    /// <param name="key">The unique key for the operation to check.</param>
    /// <param name="debounceTime">The debounce interval.</param>
    /// <param name="isFirstCall">Out parameter indicating whether this is the first call.</param>
    /// <returns>True if the operation is still within the debounce time, false otherwise.</returns>
    private bool IsDebounced(string key, TimeSpan debounceTime, out bool isFirstCall)
    {
        var cacheKey = $"Debounce-{key}";
        var lastExecuted = _memoryCache.Get<DateTimeOffset?>(cacheKey);

        if (!lastExecuted.HasValue)
        {
            isFirstCall = true;
            _memoryCache.Set(cacheKey, DateTimeOffset.UtcNow,
                new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = debounceTime });
            return false;
        }

        isFirstCall = false;
        if (DateTimeOffset.UtcNow - lastExecuted.Value < debounceTime) return true;

        _memoryCache.Set(cacheKey, DateTimeOffset.UtcNow,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = debounceTime });
        return false;
    }

    /// <summary>
    ///     Generates a unique key for debouncing purposes based on the caller's method, file path, and line number if no key
    ///     is provided.
    /// </summary>
    /// <param name="key">The provided key or an empty string if none is provided.</param>
    /// <param name="caller">The method name of the caller.</param>
    /// <param name="filePath">The file path of the caller.</param>
    /// <param name="lineNumber">The line number of the caller's call in the source code.</param>
    /// <returns>A unique key generated or modified based on the input parameters.</returns>
    private static string GenerateKey(string key, string caller, string filePath, int lineNumber)
    {
        if (string.IsNullOrEmpty(key)) key = $"{caller}-{filePath}-{lineNumber}";
        return key;
    }
}
