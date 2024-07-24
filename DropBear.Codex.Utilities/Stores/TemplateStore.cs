#region

using System.Collections.Concurrent;

#endregion

namespace DropBear.Codex.Utilities.Stores;

/// <summary>
///     Represents a template store that associates template names with their corresponding fields.
/// </summary>
/// <typeparam name="TTemplate">The type of the template name.</typeparam>
/// <typeparam name="TField">The type of the field name.</typeparam>
public class TemplateStore<TTemplate, TField> where TTemplate : notnull
{
    private readonly ConcurrentDictionary<TTemplate, List<TField>> _templateFields = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="TemplateStore{TTemplate, TField}" /> class.
    /// </summary>
    public TemplateStore() { }

    /// <summary>
    ///     Gets the number of templates in the store.
    /// </summary>
    public int Count => _templateFields.Count;

    /// <summary>
    ///     Gets a read-only collection of all the template names in the store.
    /// </summary>
    public ICollection<TTemplate> Templates => _templateFields.Keys;

    /// <summary>
    ///     Adds a template and its associated fields to the store.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="fields">The list of fields associated with the template.</param>
    /// <exception cref="ArgumentNullException">Thrown if templateName or fields is null.</exception>
#pragma warning disable CA1002
    public void AddTemplate(TTemplate templateName, List<TField> fields)
#pragma warning restore CA1002
    {
        if (templateName == null)
        {
            throw new ArgumentNullException(nameof(templateName), "Template name cannot be null.");
        }

        if (fields == null)
        {
            throw new ArgumentNullException(nameof(fields), "Fields cannot be null.");
        }

        _templateFields.TryAdd(templateName, fields);
    }

    /// <summary>
    ///     Retrieves the fields associated with a specific template.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>The list of fields associated with the template, or an empty list if the template is not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if templateName is null.</exception>
#pragma warning disable CA1002
    public List<TField> GetTemplateFields(TTemplate templateName)
#pragma warning restore CA1002
    {
        if (templateName == null)
        {
            throw new ArgumentNullException(nameof(templateName), "Template name cannot be null.");
        }

        return _templateFields.TryGetValue(templateName, out var fields) ? fields : new List<TField>();
    }

    /// <summary>
    ///     Removes a template and its associated fields from the store.
    /// </summary>
    /// <param name="templateName">The name of the template to remove.</param>
    /// <returns><c>true</c> if the template was successfully removed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if templateName is null.</exception>
    public bool RemoveTemplate(TTemplate templateName)
    {
        if (templateName == null)
        {
            throw new ArgumentNullException(nameof(templateName), "Template name cannot be null.");
        }

        return _templateFields.TryRemove(templateName, out _);
    }

    /// <summary>
    ///     Determines whether the store contains a specific template.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns><c>true</c> if the store contains the specified template; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if templateName is null.</exception>
    public bool ContainsTemplate(TTemplate templateName)
    {
        if (templateName == null)
        {
            throw new ArgumentNullException(nameof(templateName), "Template name cannot be null.");
        }

        return _templateFields.ContainsKey(templateName);
    }
}
