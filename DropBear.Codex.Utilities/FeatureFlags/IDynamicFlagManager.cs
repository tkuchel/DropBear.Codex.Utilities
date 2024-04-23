namespace DropBear.Codex.Utilities.FeatureFlags;

public interface IDynamicFlagManager
{
    /// <summary>
    ///     Adds a new flag to the manager if it does not already exist.
    /// </summary>
    void AddFlag(string flagName);

    /// <summary>
    ///     Removes a flag from the manager.
    /// </summary>
    void RemoveFlag(string flagName);

    /// <summary>
    ///     Sets a specific flag.
    /// </summary>
    void SetFlag(string flagName);

    /// <summary>
    ///     Clears a specific flag.
    /// </summary>
    void ClearFlag(string flagName);

    /// <summary>
    ///     Toggles the state of a specific flag.
    /// </summary>
    void ToggleFlag(string flagName);

    /// <summary>
    ///     Checks if a specific flag is set.
    /// </summary>
    bool IsFlagSet(string flagName);

    /// <summary>
    ///     Serializes the current state of the flag manager.
    /// </summary>
    string Serialize();

    /// <summary>
    ///     Deserializes the provided data into the flag manager.
    /// </summary>
    void Deserialize(string serializedData);

    /// <summary>
    ///     Returns a list of all flags and their current states.
    /// </summary>
    Dictionary<string, bool> GetAllFlags();
}
