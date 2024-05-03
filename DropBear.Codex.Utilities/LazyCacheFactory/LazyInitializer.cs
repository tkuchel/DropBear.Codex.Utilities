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
    /// <param name="initializationFunc">The initialization function to be used.</param>
    /// <param name="defaultValue">The default value to return if the initialization function fails or throws an error.</param>
    /// <returns>A lazy object with the specified initialization function and default value.</returns>
    public static Lazy<T> CreateDefaultLazyInitializer<T>(string functionName, Func<T> initializationFunc,
        T defaultValue = default!) =>
        new(() =>
        {
            try
            {
                return initializationFunc();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lazy initialization function for {functionName} failed: {ex.Message}");
                return defaultValue;
            }
        });

    /// <summary>
    ///     Creates a default lazy initializer for lists of a given type.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="initializationFunc">The initialization function to be used.</param>
    /// <returns>A lazy object with the specified initialization function and an empty list as the default value.</returns>
    public static Lazy<List<T>> CreateDefaultListLazyInitializer<T>(string functionName,
        Func<List<T>> initializationFunc) =>
        new(() =>
        {
            try
            {
                return initializationFunc();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Lazy initialization function for list of {typeof(T).Name} ({functionName}) failed: {ex.Message}");
                return new List<T>(); // Returns an empty list by default
            }
        });

    /// <summary>
    ///     Creates a default async lazy initializer for a given type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be asynchronously lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="taskFactory">The asynchronous task factory for initialization.</param>
    /// <param name="defaultValue">The default value to return if the initialization function fails or throws an error.</param>
    /// <returns>An async lazy object with the specified initialization function and default value.</returns>
    public static AsyncLazy<T> CreateDefaultAsyncLazyInitializer<T>(string functionName,
        Func<CancellationToken, Task<T>> taskFactory, T defaultValue = default!) =>
        new(async cancellationToken =>
        {
            try
            {
                return await taskFactory(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Async lazy initialization function for {functionName} failed: {ex.Message}");
                return defaultValue;
            }
        });

    /// <summary>
    ///     Creates a resettable lazy initializer for a given type with optional default value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="initializationFunc">The initialization function.</param>
    /// <param name="defaultValue">The default value to return if the initialization function fails or throws an error.</param>
    /// <returns>A resettable lazy object with the specified initialization function and default value.</returns>
    public static ResettableLazy<T> CreateResettableLazyInitializer<T>(string functionName, Func<T> initializationFunc,
        T defaultValue = default!) =>
        new(() =>
        {
            try
            {
                return initializationFunc();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Resettable lazy initialization function for {functionName} failed: {ex.Message}");
                return defaultValue;
            }
        }, defaultValue);

    /// <summary>
    ///     Creates an async resettable lazy initializer for a given type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be asynchronously lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="taskFactory">The asynchronous task factory for initialization.</param>
    /// <param name="defaultValue">The default value to return if the initialization function fails or throws an error.</param>
    /// <returns>An async resettable lazy object with the specified initialization function and default value.</returns>
    public static AsyncResettableLazy<T> CreateAsyncResettableLazyInitializer<T>(string functionName,
        Func<CancellationToken, Task<T>> taskFactory, T defaultValue = default!) =>
        new(async cancellationToken =>
        {
            try
            {
                return await taskFactory(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Async resettable lazy initialization function for {functionName} failed: {ex.Message}");
                return defaultValue;
            }
        });

    /// <summary>
    ///     Creates a default lazy initializer for a given type with optional default value and a custom equality comparer.
    /// </summary>
    /// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
    /// <param name="functionName">The name of the function for diagnostic purposes.</param>
    /// <param name="initializationFunc">The initialization function to be used.</param>
    /// <param name="defaultValue">The default value to return if the initialization function fails or throws an error.</param>
    /// <param name="comparer">The equality comparer to use for comparing values.</param>
    /// <returns>A lazy object with the specified initialization function, default value, and equality comparer.</returns>
    public static Lazy<T> CreateDefaultLazyInitializerWithComparer<T>(string functionName, Func<T> initializationFunc,
        T defaultValue = default!, IEqualityComparer<T>? comparer = null) =>
        new(() =>
        {
            try
            {
                var value = initializationFunc();
                if (comparer?.Equals(value, defaultValue) ?? EqualityComparer<T>.Default.Equals(value, defaultValue))
                    return defaultValue;
                return value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lazy initialization function for {functionName} failed: {ex.Message}");
                return defaultValue;
            }
        });
}
