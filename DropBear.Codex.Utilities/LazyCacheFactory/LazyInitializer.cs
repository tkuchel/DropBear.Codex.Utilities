using System.Diagnostics;
using DropBear.Codex.Utilities.LazyCacheFactory.Models;

namespace DropBear.Codex.Utilities.LazyCacheFactory;

/// <summary>
///     Provides methods to create default lazy initializers for different types of lazy instances.
/// </summary>
public static class LazyInitializer
{
    /// <summary>
    ///     Creates a default lazy initializer for a given type with optional default value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="defaultValue">The default value to return if the initialization function is not set.</param>
    /// <returns>A function that initializes the lazy value.</returns>
    public static Func<T> CreateDefaultLazyInitializer<T>(string functionName, T defaultValue = default!) =>
        () =>
        {
            Debug.WriteLine($"Lazy initialization function for {functionName} is not set.");
            return defaultValue;
        };

    /// <summary>
    ///     Creates a default lazy initializer for lists of a given type.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <returns>A function that initializes the lazy list.</returns>
    public static Func<List<T>> CreateDefaultListLazyInitializer<T>(string functionName) =>
        () =>
        {
            Debug.WriteLine($"Lazy initialization function for list of {typeof(T).Name} ({functionName}) is not set.");
            return []; // Returns an empty list by default
        };

    /// <summary>
    ///     Creates a resettable lazy initializer for a given type with optional default value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="defaultValue">The default value to return if the initialization function is not set.</param>
    /// <returns>A function that initializes the resettable lazy value.</returns>
    public static Func<T> CreateResettableLazyInitializer<T>(string functionName, T defaultValue = default!) =>
        () =>
        {
            Debug.WriteLine($"Resettable lazy initialization function for {functionName} is not set.");
            return defaultValue;
        };

    /// <summary>
    ///     Creates a default async lazy initializer for a given type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be asynchronously lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="taskFactory">The asynchronous task factory if initialization is not set.</param>
    /// <returns>A function that initializes the async lazy value.</returns>
    public static Func<AsyncLazy<T>> CreateDefaultAsyncLazyInitializer<T>(string functionName,
        Func<Task<T>> taskFactory = default!) =>
        () =>
        {
            Debug.WriteLine($"Async lazy initialization function for {functionName} is not set.");
            if (taskFactory is null)
                throw new InvalidOperationException(
                    $"Async initialization function for {functionName} must be provided.");
            return new AsyncLazy<T>(taskFactory);
        };
}
