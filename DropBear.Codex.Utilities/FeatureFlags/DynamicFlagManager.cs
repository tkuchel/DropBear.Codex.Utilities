namespace DropBear.Codex.Utilities.FeatureFlags;

public class DynamicFlagManager : IDynamicFlagManager
{
    private readonly Dictionary<string, int> _featureMap = new(StringComparer.OrdinalIgnoreCase);
    private int _flags;
    private int _nextFreeBit;

    public void AddFeature(string featureName)
    {
        if (_featureMap.ContainsKey(featureName) || _nextFreeBit >= 32) return;
        _featureMap[featureName] = 1 << _nextFreeBit;
        _nextFreeBit++;
    }

    public void RemoveFeature(string featureName)
    {
        if (!_featureMap.TryGetValue(featureName, out var bitValue)) return;
        _flags &= ~bitValue; // Clear the flag if it's set
        _featureMap.Remove(featureName);
        // Note: This does not reorganize the bit positions
    }

    public void SetFlag(string featureName)
    {
        if (_featureMap.TryGetValue(featureName, out var bitValue)) _flags |= bitValue;
    }

    public void ClearFlag(string featureName)
    {
        if (_featureMap.TryGetValue(featureName, out var bitValue)) _flags &= ~bitValue;
    }

    public void ToggleFlag(string featureName)
    {
        if (_featureMap.TryGetValue(featureName, out var bitValue)) _flags ^= bitValue;
    }

    public bool IsFlagSet(string featureName) =>
        _featureMap.TryGetValue(featureName, out var bitValue) && (_flags & bitValue) == bitValue;
}
