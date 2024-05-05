namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

/// <summary>
/// Provides a resettable lazy initialization pattern.
/// </summary>
/// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
public class ResettableLazy<T>
{
    private readonly object _lock = new();
    private readonly Func<T> _valueFactory;
    private T _value;
    private volatile bool _isValueCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with the specified value factory.
    /// </summary>
    /// <param name="valueFactory">The factory function that creates the value.</param>
    /// <param name="defaultValue">The default value to be returned if the value factory throws an exception.</param>
    public ResettableLazy(Func<T> valueFactory, T defaultValue = default!)
    {
        _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        _value = defaultValue;
        DefaultValue = defaultValue;
        _isValueCreated = false;
    }

    /// <summary>
    /// Gets the lazily initialized value, creating it if necessary.
    /// </summary>
    public T Value
    {
        get
        {
            if (_isValueCreated) return _value;
            lock (_lock)
            {
                if (_isValueCreated) return _value;
                try
                {
                    _value = _valueFactory();
                    _isValueCreated = true;
                }
                catch
                {
                    _value = DefaultValue; // Consider logging this exception or handling it differently
                    _isValueCreated = false; // Consider whether to mark it as not created
                    throw; // Rethrowing the exception maintains the stack trace
                }
            }

            return _value;
        }
    }

    /// <summary>
    /// Gets the default value to be returned if the value factory throws an exception.
    /// </summary>
    public T DefaultValue { get; }

    /// <summary>
    /// Gets a value indicating whether the value has been created.
    /// </summary>
    public bool IsValueCreated
    {
        get
        {
            lock (_lock)
            {
                return _isValueCreated;
            }
        }
    }


    /// <summary>
    /// Resets the lazy value, allowing it to be recreated on the next access.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _isValueCreated = false;
            _value = DefaultValue; // Reset the value to the default value
        }
    }

    /// <summary>
    /// Tries to get the lazily initialized value without causing initialization.
    /// </summary>
    /// <param name="value">The current value, if it has been created.</param>
    /// <returns><c>true</c> if the value has been created; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(out T value)
    {
        lock (_lock) // Ensure thread safety on read
        {
            value = _isValueCreated ? _value : DefaultValue;
            return _isValueCreated;
        }
    }
}