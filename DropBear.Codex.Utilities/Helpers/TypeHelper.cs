namespace DropBear.Codex.Utilities.Helpers;

public static class TypeHelper
{
    /// <summary>
    /// Determines whether the specified type is of the specified target type or derives from it.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="targetType">The target type to compare against.</param>
    /// <returns><see langword="true"/> if <paramref name="typeToCheck"/> is of type <paramref name="targetType"/> or derives from it; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="typeToCheck"/> or <paramref name="targetType"/> is null.</exception>
    public static bool IsOfTypeOrDerivedFrom(Type typeToCheck, Type targetType)
    {
        // Ensuring non-null arguments to prevent null reference exceptions
        if (typeToCheck == null)
        {
            throw new ArgumentNullException(nameof(typeToCheck), "The type to check cannot be null.");
        }

        if (targetType == null)
        {
            throw new ArgumentNullException(nameof(targetType), "The target type cannot be null.");
        }

        // Direct type comparison for performance before walking the hierarchy
        if (typeToCheck == targetType || targetType.IsAssignableFrom(typeToCheck))
        {
            return true;
        }

        // Walking the hierarchy if direct comparison and IsAssignableFrom check do not yield result
        Type? currentType = typeToCheck.BaseType;
        while (currentType != null && currentType != typeof(object))
        {
            if (currentType == targetType)
            {
                return true;
            }
            currentType = currentType.BaseType;
        }

        return false;
    }
}

