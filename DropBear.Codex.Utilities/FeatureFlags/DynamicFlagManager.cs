namespace DropBear.Codex.Utilities.FeatureFlags;

public class DynamicFlagManager : IDynamicFlagManager
{
    private readonly Dictionary<string, int> _flagMap = new(StringComparer.OrdinalIgnoreCase);
    private int _flags;
    private int _nextFreeBit;

    // Overload for adding flags by enum
    public void AddFlag<TEnum>(TEnum flag) where TEnum : Enum
    {
        AddFlag(flag.ToString());
    }

    public void AddFlag(string flagName)
    {
        if (_flagMap.ContainsKey(flagName) || _nextFreeBit >= 32) return;
        _flagMap[flagName] = 1 << _nextFreeBit;
        _nextFreeBit++;
    }

    // Overload for removing flags by enum
    public void RemoveFlag<TEnum>(TEnum flag) where TEnum : Enum
    {
        RemoveFlag(flag.ToString());
    }

    public void RemoveFlag(string flagName)
    {
        if (!_flagMap.TryGetValue(flagName, out var bitValue)) return;
        _flags &= ~bitValue; // Clear the flag if it's set
        _flagMap.Remove(flagName);
        // Note: This does not reorganize the bit positions
    }

    // Overload for setting flags by enum
    public void SetFlag<TEnum>(TEnum flag) where TEnum : Enum
    {
        SetFlag(flag.ToString());
    }

    public void SetFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue)) _flags |= bitValue;
    }

    // Overload for clearing flags by enum
    public void ClearFlag<TEnum>(TEnum flag) where TEnum : Enum
    {
        ClearFlag(flag.ToString());
    }

    public void ClearFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue)) _flags &= ~bitValue;
    }

    // Overload for toggling flags by enum
    public void ToggleFlag<TEnum>(TEnum flag) where TEnum : Enum
    {
        ToggleFlag(flag.ToString());
    }

    public void ToggleFlag(string flagName)
    {
        if (_flagMap.TryGetValue(flagName, out var bitValue)) _flags ^= bitValue;
    }

    // Overload for checking if a flag is set by enum
    public bool IsFlagSet<TEnum>(TEnum flag) where TEnum : Enum
    {
        return IsFlagSet(flag.ToString());
    }

    public bool IsFlagSet(string flagName) =>
        _flagMap.TryGetValue(flagName, out var bitValue) && (_flags & bitValue) == bitValue;
}
