using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests.Caches;

[TestClass]
public class MemoryCacheTest
{
    [TestMethod]
    public void TestMemoryCacheAdd()
    {
        // Prepare
        var cache = new MemoryCache();
        var key = "Key";
        var value = new object();

        // Act
        cache.Add(key, value);

        // Assert
        Assert.AreEqual(1, cache.OfType<object>().Count());
    }

    [TestMethod]
    public void TestMemoryCacheCacheItem()
    {
        // Prepare
        var cache = new MemoryCache();
        var item = new CacheItem<object>("Key", new object());

        // Act
        cache.Add(item);

        // Assert
        Assert.AreEqual(1, cache.OfType<object>().Count());
    }

    [TestMethod]
    public void TestMemoryCacheClear()
    {
        // Prepare
        var cache = new MemoryCache
        {
            new CacheItem<object>("Key", new object())
        };

        // Act
        cache.Clear();

        // Assert
        Assert.IsFalse(cache.OfType<object>().Any());
    }

    [TestMethod]
    public void TestMemoryCacheContains()
    {
        // Prepare
        var cache = new MemoryCache
        {
            new CacheItem<object>("Key", new object())
        };

        // Act
        var actual = cache.Contains("Key");

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void TestMemoryCacheGet()
    {
        // Prepare
        var cache = new MemoryCache();
        var key = "Key";
        var value = new object();
        cache.Add(key, value);

        // Act
        var actual = cache.Get<object>(key);

        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(value, actual.Value);
    }

    [TestMethod]
    public void TestMemoryCacheGetExpired()
    {
        // Prepare
        var cache = new MemoryCache
        {
            { "Key", new object(), 0 }
        };

        // Act
        var actual = cache.Get<object>("Key");

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void TestMemoryCacheOverrideExpiredItem()
    {
        // Prepare
        var cache = new MemoryCache
        {
            { "Key", new object(), 0 }
        };

        // Act
        var actual = cache.Get<object>("Key");

        // Assert
        Assert.IsNull(actual);

        // Reprepare
        cache.Add("Key", new object());

        // React
        actual = cache.Get<object>("Key");

        // Reassert
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void TestMemoryCacheManualExpirationDate()
    {
        // Prepare
        var cache = new MemoryCache();
        var expirationInMinutes = 60;
        cache.Add("Key", new object(), expirationInMinutes);

        // Act
        var actual = cache.Get<object>("Key");

        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expirationInMinutes, (actual.Expiration - actual.CreatedDate).TotalMinutes);
    }

    [TestMethod]
    public void TestMemoryCacheManualExpirationDateViaCacheItem()
    {
        // Prepare
        var cache = new MemoryCache();
        var expirationInMinutes = 60;
        var cacheItem = new CacheItem<object>("Key", new object(), expirationInMinutes);
        cache.Add(cacheItem);

        // Act
        var actual = cache.Get<object>("Key");

        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expirationInMinutes, (actual.Expiration - actual.CreatedDate).TotalMinutes);
    }

    [TestMethod]
    public void ThrowExceptionOnAddingNewItemAtMemoryCacheWithTheSameKey()
    {
        // Prepare
        var cache = new MemoryCache();
        cache.Add("Key", new object(), throwException: true);

        // Act/Assert
        Assert.ThrowsExactly<MappingExistsException>(() => cache.Add("Key", new object(), throwException: true));
    }

    [TestMethod]
    public void ThrowExceptionAtMemoryCacheOnAddingACacheWithNegativeExpiration()
    {
        // Prepare
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new MemoryCache
        {
            // Act/Assert
            { "Key", "Value", -1 }
        });
    }

    [TestMethod]
    public void ThrowExceptionAtMemoryCacheOnRemovingAKeyThatIsNotPresent()
    {
        // Prepare
        var cache = new MemoryCache();

        // Act/Assert
        Assert.ThrowsExactly<ItemNotFoundException>(() => cache.Remove("Key", true));
    }
}
