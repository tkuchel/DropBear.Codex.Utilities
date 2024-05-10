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
    /// </summary>
    /// <param name="memoryCache">The memory cache used to store timestamps for debouncing.</param>
    public DebounceService(IMemoryCache memoryCache) => _memoryCache = memoryCache;


    /// <summary>
    ///     Debounces a function returning a generic Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="key">A unique key identifying the function call for debouncing purposes.</param>
    /// <param name="debounceTime">The minimum time interval between successive executions.</param>
    /// <param name="caller"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function execution.</returns>
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

        if (IsDebounced(key, debounceTime.Value)) return Result<T>.Failure("Operation debounced.");

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
    ///     Debounces an action returning a non-generic Result.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="key">A unique key identifying the action call for debouncing purposes.</param>
    /// <param name="debounceTime">The minimum time interval between successive executions.</param>
    /// <param name="caller"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the action execution.</returns>
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

        if (IsDebounced(key, debounceTime.Value)) return Result.Failure("Operation debounced.");

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
    ///     Checks if a given key has been debounced within the specified timeframe.
    /// </summary>
    /// <param name="key">The unique key for the operation to check.</param>
    /// <param name="debounceTime">The debounce interval.</param>
    /// <returns>True if the operation is still within the debounce time, false otherwise.</returns>
    private bool IsDebounced(string key, TimeSpan debounceTime)
    {
        var cacheKey = $"Debounce-{key}";
        var lastExecuted = _memoryCache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = debounceTime;
            return DateTimeOffset.UtcNow;
        });

        if (DateTimeOffset.UtcNow - lastExecuted >= debounceTime) return false;

        _memoryCache.Set(cacheKey, DateTimeOffset.UtcNow,
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = debounceTime });
        return true;
    }


    private static string GenerateKey(string key, string caller, string filePath, int lineNumber)
    {
        if (string.IsNullOrEmpty(key)) key = $"{caller}-{filePath}-{lineNumber}";
        return key;
    }
}
