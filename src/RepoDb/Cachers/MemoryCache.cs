using System.Collections;
using System.Collections.Concurrent;
using RepoDb.Exceptions;
using RepoDb.Interfaces;

namespace RepoDb;

/// <summary>
/// A class object that is used for caching the resultsets of the query operations. This is the default cache object used by the <see cref="DbRepository{TDbConnection}"/> and <see cref="BaseRepository{T, TDbConnection}"/> repository objects.
/// </summary>
#pragma warning disable CA1010 // Generic interface should also be implemented
public class MemoryCache : ICache
{
    private readonly ConcurrentDictionary<string, IExpirable> _cache = new();

    #region Sync

    /// <summary>
    /// Adds a cache item value.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="key">The key to the cache.</param>
    /// <param name="value">The value of the cache.</param>
    /// <param name="expiration">The expiration in minutes of the cache item.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
    public void Add<T>(string key,
        T value,
        int expiration = Constant.DefaultCacheItemExpirationInMinutes,
        bool throwException = true) =>
        Add(new CacheItem<T>(key, value, expiration), throwException);

    /// <summary>
    /// Adds a cache item value.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="item">The cache item to be added in the collection.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
    public void Add<T>(CacheItem<T> item,
        bool throwException = true)
    {
        ArgumentNullException.ThrowIfNull(item);
        CacheItem<T>? cacheItem = null;
        if (_cache.TryGetValue(item.Key, out var value))
        {
            cacheItem = value as CacheItem<T>;
        }
        if (cacheItem == null)
        {
            if (_cache.TryAdd(item.Key, item) == false && throwException == true)
            {
                throw new InvalidOperationException($"Fail to add an item into the cache for the key {item.Key}.");
            }
        }
        else
        {
            if (!cacheItem.IsExpired() && throwException == true)
            {
                throw new MappingExistsException($"An existing cache for key '{item.Key}' already exists.");
            }
            cacheItem.Update(item, throwException);
        }
    }

    /// <summary>
    /// Clears the collection of the cache.
    /// </summary>
    public void Clear() =>
        _cache.Clear();

    /// <summary>
    /// Checks whether the key is present in the collection.
    /// </summary>
    /// <param name="key">The name of the key to be checked.</param>
    /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
    public bool Contains(string key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            if (!value.IsExpired())
                return true;

            _cache.TryRemove(key, out var _);
        }
        return false;
    }

    /// <summary>
    /// Gets an object from the cache collection.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="key">The key of the cache object to be retrieved.</param>
    /// <returns>A cached item object from the cache collection based on the given key.</returns>
    /// <param name="throwException">Throws an exception if the item is not found.</param>
    public CacheItem<T>? Get<T>(string key,
        bool throwException = true)
    {
        CacheItem<T>? item = null;
        if (_cache.TryGetValue(key, out var value))
        {
            item = value as CacheItem<T>;
        }
        if (item?.IsExpired() == false)
        {
            return item;
        }
        _cache.TryRemove(key, out var _);
        return null;
    }

    /// <summary>
    /// Removes the item from the cache collection.
    /// </summary>
    /// <param name="key">The key of the item to be removed from the cache collection.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
    public bool Remove(string key,
        bool throwException = true)
    {
        bool removed = _cache.TryRemove(key, out var _);

        if (removed)
            return true;
        else if (throwException == true)
            throw new ItemNotFoundException($"Failed to remove an item with key '{key}'.");

        return false;
    }

    #endregion

    #region Async

    /// <summary>
    /// Adds a cache item value in an asynchronous way.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="key">The key to the cache.</param>
    /// <param name="value">The value of the cache.</param>
    /// <param name="expiration">The expiration in minutes of the cache item.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    public ValueTask AddAsync<T>(string key,
        T value,
        int expiration = Constant.DefaultCacheItemExpirationInMinutes,
        bool throwException = true,
        CancellationToken cancellationToken = default) =>
        AddAsync(new CacheItem<T>(key, value, expiration), throwException, cancellationToken);

    /// <summary>
    /// Adds a cache item value in an asynchronous way.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="item">The cache item to be added in the collection.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to add an item.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    public ValueTask AddAsync<T>(CacheItem<T> item,
        bool throwException = true,
        CancellationToken cancellationToken = default)
    {
        Add(item, throwException);
        return new();
    }

    /// <summary>
    /// Clears the collection of the cache in an asynchronous way.
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// </summary>
    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        _cache.Clear();
        return new();
    }

    /// <summary>
    /// Checks whether the key is present in the collection in an asynchronous way.
    /// </summary>
    /// <param name="key">The name of the key to be checked.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
    public ValueTask<bool> ContainsAsync(string key,
        CancellationToken cancellationToken = default) =>
        new(Contains(key));

    /// <summary>
    /// Gets an object from the cache collection in an asynchronous way.
    /// </summary>
    /// <typeparam name="T">The type of the cache item value.</typeparam>
    /// <param name="throwException">Throws an exception if the item is not found.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <param name="key">The key of the cache object to be retrieved.</param>
    /// <returns>A cached item object from the cache collection based on the given key.</returns>
    public ValueTask<CacheItem<T>?> GetAsync<T>(string key,
        bool throwException = true,
        CancellationToken cancellationToken = default) =>
        new(Get<T>(key, throwException));

    /// <summary>
    /// Removes the item from the cache collection in an asynchronous way.
    /// </summary>
    /// <param name="key">The key of the item to be removed from the cache collection.</param>
    /// <param name="throwException">Throws an exception if the operation has failed to remove an item.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    public ValueTask<bool> RemoveAsync(string key,
        bool throwException = true,
        CancellationToken cancellationToken = default)
    {
        return new(Remove(key, throwException));
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the enumerator of the cache collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <summary>
    /// Gets the enumerator of the cache collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator() =>
        _cache
            .Where(kvp => (kvp.Value as IExpirable)?.IsExpired() == false)
            .Select(kvp => kvp.Value)
            .GetEnumerator();

    #endregion
}
