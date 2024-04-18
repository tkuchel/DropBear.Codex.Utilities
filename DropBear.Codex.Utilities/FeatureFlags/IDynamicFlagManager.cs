namespace DropBear.Codex.Utilities.FeatureFlags;

/// <summary>
///     Defines a contract for managing dynamic flags.
/// </summary>
public interface IDynamicFlagManager
{
    /// <summary>
    ///     Adds a new flag with a unique name to the manager.
    /// </summary>
    /// <param name="flagName">The name of the to add.</param>
    void AddFlag(string flagName);

    /// <summary>
    ///     Adds a new flag using an enum value to the manager.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to add.</param>
    void AddFlag<TEnum>(TEnum flag) where TEnum : Enum;

    /// <summary>
    ///     Removes an existing flag by name from the manager.
    /// </summary>
    /// <param name="flagName">The name of the to remove.</param>
    void RemoveFlag(string flagName);

    /// <summary>
    ///     Removes an existing flag using an enum value from the manager.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to remove.</param>
    void RemoveFlag<TEnum>(TEnum flag) where TEnum : Enum;

    /// <summary>
    ///     Sets a flag to the 'on' state.
    /// </summary>
    /// <param name="flagName">The name of the to set.</param>
    void SetFlag(string flagName);

    /// <summary>
    ///     Sets a flag using an enum value to the 'on' state.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to set.</param>
    void SetFlag<TEnum>(TEnum flag) where TEnum : Enum;

    /// <summary>
    ///     Clears a flag to the 'off' state.
    /// </summary>
    /// <param name="flagName">The name of the to clear.</param>
    void ClearFlag(string flagName);

    /// <summary>
    ///     Clears a flag using an enum value to the 'off' state.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to clear.</param>
    void ClearFlag<TEnum>(TEnum flag) where TEnum : Enum;

    /// <summary>
    ///     Toggles the current state of a flag.
    /// </summary>
    /// <param name="flagName">The name of the to toggle.</param>
    void ToggleFlag(string flagName);

    /// <summary>
    ///     Toggles the current state of a flag using an enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to toggle.</param>
    void ToggleFlag<TEnum>(TEnum flag) where TEnum : Enum;

    /// <summary>
    ///     Checks if a flag is currently set to the 'on' state.
    /// </summary>
    /// <param name="flagName">The name of the to check.</param>
    /// <returns>True if the is on, otherwise false.</returns>
    bool IsFlagSet(string flagName);

    /// <summary>
    ///     Checks if a flag is currently set to the 'on' state using an enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type defining the flag.</typeparam>
    /// <param name="flag">The enum value of the flag to check.</param>
    /// <returns>True if the is on, otherwise false.</returns>
    bool IsFlagSet<TEnum>(TEnum flag) where TEnum : Enum;
}
