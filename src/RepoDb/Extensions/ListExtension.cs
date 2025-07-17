namespace RepoDb.Extensions;

/// <summary>
/// An extension class for <see cref="IList{T}"/>.
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// Adds an item into the <see cref="ICollection{T}"/> if not null.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="list">The instance of the list.</param>
    /// <param name="item">The item to be evaulated and added.</param>
    public static void AddIfNotNull<T>(this ICollection<T> list,
        T? item)
    {
        ArgumentNullException.ThrowIfNull(list);
        if (item != null)
        {
            list.Add(item);
        }
    }

    /// <summary>
    /// Adds an item into the <see cref="List{T}"/> if not null.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="list">The instance of the list.</param>
    /// <param name="items">The items to be evaulated and added.</param>
    public static void AddRangeIfNotNullOrNotEmpty<T>(this List<T> list,
        IEnumerable<T>? items)
    {
        ArgumentNullException.ThrowIfNull(list);
        if (items?.Any() == true)
        {
            list.AddRange(items);
        }
    }
}
