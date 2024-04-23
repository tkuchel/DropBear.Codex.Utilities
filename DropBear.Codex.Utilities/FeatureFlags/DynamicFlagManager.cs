using System.Collections.Concurrent;
using System.Text.Json;

namespace DropBear.Codex.Utilities.FeatureFlags;

public class DynamicFlagManager : IDynamicFlagManager
{
    private readonly ConcurrentDictionary<string, bool> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _flagMap = new(StringComparer.OrdinalIgnoreCase);
    private int _flags;
    private int _nextFreeBit;

    /// <summary>
    ///     Adds a new flag to the manager if it does not already exist.
    /// </summary>
    public void AddFlag(string flagName)
    {
        if (_flagMap.ContainsKey(flagName) || _nextFreeBit >= 32)
            throw new InvalidOperationException("Flag already exists or limit exceeded.");

        _flagMap[flagName] = 1 << _nextFreeBit++;
        _cache.Clear(); // Reset the cache to ensure consistency.
    }

    /// <summary>
    ///     Removes a flag from the manager.
    /// </summary>
    public void RemoveFlag(string flagName)
    {
        if (_flagMap.TryRemove(flagName, out var bitValue))
        {
            _flags &= ~bitValue;
            _cache.Clear();
        }
        else
        {
            throw new KeyNotFoundException("Flag not found.");
        }
    }

    /// <summary>
    ///     Sets a specific flag.
    /// </summary>
    public void SetFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags |= bitValue;
            _cache[flagName] = true;
        }
        else
        {
            throw new KeyNotFoundException("Flag not found.");
        }
    }

    /// <summary>
    ///     Clears a specific flag.
    /// </summary>
    public void ClearFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags &= ~bitValue;
            _cache[flagName] = false;
        }
        else
        {
            throw new KeyNotFoundException("Flag not found.");
        }
    }

    /// <summary>
    ///     Toggles the state of a specific flag.
    /// </summary>
    public void ToggleFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags ^= bitValue;
            _cache[flagName] = (_flags & bitValue) == bitValue;
        }
        else
        {
            throw new KeyNotFoundException("Flag not found.");
        }
    }

    /// <summary>
    ///     Checks if a specific flag is set.
    /// </summary>
    public bool IsFlagSet(string flagName)
    {
        if (_cache.TryGetValue(flagName, out var isSet))
            return isSet;

        if (!_flagMap.TryGetValue(flagName, out var bitValue))
            throw new KeyNotFoundException("Flag not found.");

        isSet = (_flags & bitValue) == bitValue;
        _cache[flagName] = isSet; // Cache the result.
        return isSet;
    }

    /// <summary>
    ///     Serializes the current state of the flag manager.
    /// </summary>
    public string Serialize()
    {
        var data = new SerializationData { Flags = _flagMap, CurrentState = _flags, NextFreeBit = _nextFreeBit };
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    ///     Deserializes the provided data into the flag manager.
    /// </summary>
    public void Deserialize(string serializedData)
    {
        var data = JsonSerializer.Deserialize<SerializationData>(serializedData);
        if (data is null)
            throw new InvalidOperationException("Failed to deserialize flag data.");

        _flagMap.Clear();
        foreach (var (key, value) in data.Flags)
            _flagMap[key] = value;

        _flags = data.CurrentState;
        _nextFreeBit = data.NextFreeBit;
        _cache.Clear();
    }

    /// <summary>
    ///     Returns a list of all flags and their current states.
    /// </summary>
    public Dictionary<string, bool> GetAllFlags() => _flagMap.Keys.ToDictionary(flag => flag, IsFlagSet, StringComparer.OrdinalIgnoreCase);

    private class SerializationData
    {
        public ConcurrentDictionary<string, int> Flags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int CurrentState { get; set; }
        public int NextFreeBit { get; set; }
    }
}
