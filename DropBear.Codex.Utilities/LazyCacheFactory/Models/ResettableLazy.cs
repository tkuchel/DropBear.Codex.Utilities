namespace DropBear.Codex.Utilities.LazyCacheFactory.Models;

public class ResettableLazy<T>
{
    private readonly object _lock = new();
    private readonly Func<T> _valueFactory;
    private T _value;

    public ResettableLazy(Func<T> valueFactory)
    {
        _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        _value = default!;
        IsValueCreated = false;
    }

    public T Value
    {
        get
        {
            if (IsValueCreated) return _value;
            lock (_lock)
            {
                if (IsValueCreated) return _value;
                _value = _valueFactory();
                IsValueCreated = true;
            }

            return _value;
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsValueCreated { get; private set; }

    public void Reset()
    {
        lock (_lock)
        {
            IsValueCreated = false;
            _value = default!; // Optionally reset _value to default
        }
    }
}
