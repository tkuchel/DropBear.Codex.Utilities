using System.Collections.Concurrent;
using System.Text.Json;
using DropBear.Codex.AppLogger.Builders;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace DropBear.Codex.Utilities.FeatureFlags;

/// <summary>
///     Manages dynamic feature flags with thread-safe operations and logging.
/// </summary>
public class DynamicFlagManager : IDynamicFlagManager
{
    private readonly ConcurrentDictionary<string, bool> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _flagMap = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<DynamicFlagManager> _logger;
    private int _flags;
    private int _nextFreeBit;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DynamicFlagManager" /> class.
    /// </summary>
    public DynamicFlagManager()
    {
        _flags = 0;
        _nextFreeBit = 0;

        var loggerFactory = new LoggerConfigurationBuilder()
            .SetLogLevel(LogLevel.Information)
            .EnableConsoleOutput()
            .Build();

        _logger = loggerFactory.CreateLogger<DynamicFlagManager>();
    }

    /// <summary>
    ///     Adds a new flag to the manager if it does not already exist.
    /// </summary>
    /// <param name="flagName">The name of the flag to add.</param>
    /// <exception cref="InvalidOperationException">Thrown if the flag already exists or the limit is exceeded.</exception>
    public void AddFlag(string flagName)
    {
        if (_flagMap.ContainsKey(flagName) || _nextFreeBit >= 32)
            LogErrorAndThrow("Flag already exists or limit exceeded.");

        _flagMap[flagName] = 1 << _nextFreeBit++;
        _cache.Clear(); // Reset the cache to ensure consistency.
        _logger.ZLogInformation($"Flag {flagName} added.");
    }

    /// <summary>
    ///     Removes a flag from the manager.
    /// </summary>
    /// <param name="flagName">The name of the flag to remove.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the flag does not exist.</exception>
    public void RemoveFlag(string flagName)
    {
        if (_flagMap.TryRemove(flagName, out var bitValue))
        {
            _flags &= ~bitValue;
            _cache.Clear();
            _logger.ZLogInformation($"Flag {flagName} removed.");
        }
        else
        {
            LogErrorAndThrow("Flag not found.");
        }
    }

    /// <summary>
    ///     Sets a specific flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to set.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the flag does not exist.</exception>
    public void SetFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags |= bitValue;
            _cache[flagName] = true;
            _logger.ZLogInformation($"Flag {flagName} set.");
        }
        else
        {
            LogErrorAndThrow("Flag not found.");
        }
    }

    /// <summary>
    ///     Clears a specific flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to clear.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the flag does not exist.</exception>
    public void ClearFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags &= ~bitValue;
            _cache[flagName] = false;
            _logger.ZLogInformation($"Flag {flagName} cleared.");
        }
        else
        {
            LogErrorAndThrow("Flag not found.");
        }
    }

    /// <summary>
    ///     Toggles the state of a specific flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to toggle.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the flag does not exist.</exception>
    public void ToggleFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue))
        {
            _flags ^= bitValue;
            _cache[flagName] = (_flags & bitValue) == bitValue;
            _logger.ZLogInformation($"Flag {flagName} toggled.");
        }
        else
        {
            LogErrorAndThrow("Flag not found.");
        }
    }

    /// <summary>
    ///     Checks if a specific flag is set.
    /// </summary>
    /// <param name="flagName">The name of the flag to check.</param>
    /// <returns>True if the flag is set; otherwise, false.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the flag does not exist.</exception>
    public bool IsFlagSet(string flagName)
    {
        if (_cache.TryGetValue(flagName, out var isSet))
            return isSet;

        if (!_flagMap.TryGetValue(flagName, out var bitValue)) LogErrorAndThrow("Flag not found.");

        isSet = (_flags & bitValue) == bitValue;
        _cache[flagName] = isSet; // Cache the result.

        return isSet;
    }

    /// <summary>
    ///     Serializes the current state of the flag manager.
    /// </summary>
    /// <returns>A JSON string representing the serialized state.</returns>
    public string Serialize()
    {
        var data = new SerializationData
        {
            Flags = _flagMap.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
            CurrentState = _flags,
            NextFreeBit = _nextFreeBit
        };

        _logger.ZLogInformation($"Flag data serialized.");
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    ///     Deserializes the provided data into the flag manager.
    /// </summary>
    /// <param name="serializedData">The JSON string representing the serialized state.</param>
    /// <exception cref="InvalidOperationException">Thrown if deserialization fails.</exception>
    public void Deserialize(string serializedData)
    {
        var data = JsonSerializer.Deserialize<SerializationData>(serializedData);
        if (data is null) LogErrorAndThrow("Failed to deserialize flag data.");

        _flagMap.Clear();
        foreach (var (key, value) in data.Flags) _flagMap[key] = value;

        _flags = data.CurrentState;
        _nextFreeBit = data.NextFreeBit;
        _cache.Clear();
        _logger.ZLogInformation($"Flag data deserialized.");
    }

    /// <summary>
    ///     Returns a list of all flags and their current states.
    /// </summary>
    /// <returns>A dictionary with flag names as keys and their states as values.</returns>
    public Dictionary<string, bool> GetAllFlags() =>
        _flagMap.Keys.ToDictionary(flag => flag, IsFlagSet, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Logs an error message and throws an exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <exception cref="InvalidOperationException">Throws with the provided message.</exception>
    private void LogErrorAndThrow(string message)
    {
        _logger.ZLogError($"{message}");
        throw new InvalidOperationException(message);
    }

    private sealed class SerializationData
    {
        public Dictionary<string, int> Flags { get; set; }
        public int CurrentState { get; set; }
        public int NextFreeBit { get; set; }
    }
}
