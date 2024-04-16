namespace DropBear.Codex.Utilities.FeatureFlags;

/// <summary>
///     Defines a contract for managing dynamic feature flags.
/// </summary>
public interface IDynamicFlagManager
{
    /// <summary>
    ///     Adds a new feature flag with a unique name to the manager.
    /// </summary>
    /// <param name="featureName">The name of the feature to add.</param>
    void AddFlag(string featureName);

    /// <summary>
    ///     Removes an existing feature flag by name from the manager.
    /// </summary>
    /// <param name="featureName">The name of the feature to remove.</param>
    void RemoveFlag(string featureName);

    /// <summary>
    ///     Sets a feature flag to the 'on' state.
    /// </summary>
    /// <param name="featureName">The name of the feature to set.</param>
    void SetFlag(string featureName);

    /// <summary>
    ///     Clears a feature flag to the 'off' state.
    /// </summary>
    /// <param name="featureName">The name of the feature to clear.</param>
    void ClearFlag(string featureName);

    /// <summary>
    ///     Toggles the current state of a feature flag.
    /// </summary>
    /// <param name="featureName">The name of the feature to toggle.</param>
    void ToggleFlag(string featureName);

    /// <summary>
    ///     Checks if a feature flag is currently set to the 'on' state.
    /// </summary>
    /// <param name="featureName">The name of the feature to check.</param>
    /// <returns>True if the feature is on, otherwise false.</returns>
    bool IsFlagSet(string featureName);
}
