using ECommerceAPI.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerceAPI.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T Get<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T value);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expirationTime)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime,
            SlidingExpiration = TimeSpan.FromMinutes(20)
        };

        _memoryCache.Set(key, value, cacheEntryOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }
}
