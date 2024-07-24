#region

using System.Collections.ObjectModel;

#endregion

namespace DropBear.Codex.Utilities.Helpers;

/// <summary>
///     Provides extension methods for working with <see cref="List{T}" /> instances.
/// </summary>
public static class ListHelper
{
    /// <summary>
    ///     Creates a read-only wrapper around the specified <see cref="List{T}" /> instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The source <see cref="List{T}" /> instance.</param>
    /// <returns>A <see cref="ReadOnlyCollection{T}" /> that wraps the source list.</returns>
    public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this List<T> list)
    {
        return new ReadOnlyCollection<T>(list);
    }
}
