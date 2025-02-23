namespace ECommerceAPI.Core.Interfaces;

/// <summary>
/// Interface for caching service operations.
/// Provides methods for storing and retrieving data from a cache system.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from the cache by key
    /// </summary>
    /// <typeparam name="T">Type of the cached value</typeparam>
    /// <param name="key">Key to lookup in the cache</param>
    /// <returns>The cached value if found, null otherwise</returns>
    T Get<T>(string key);

    /// <summary>
    /// Sets a value in the cache with the specified key and expiration time
    /// </summary>
    /// <typeparam name="T">Type of the value to cache</typeparam>
    /// <param name="key">Key to store the value under</param>
    /// <param name="value">Value to store in the cache</param>
    /// <param name="expirationTime">Time after which the cached value should expire</param>
    void Set<T>(string key, T value, TimeSpan expirationTime);

    /// <summary>
    /// Removes a value from the cache by key
    /// </summary>
    /// <param name="key">Key of the value to remove</param>
    void Remove(string key);

    /// <summary>
    /// Checks if a value exists in the cache by key
    /// </summary>
    /// <param name="key">Key to check in the cache</param>
    /// <returns>True if the key exists in the cache, false otherwise</returns>
    bool Exists(string key);
}
