using Astronomic_Catalogs.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace Astronomic_Catalogs.Services;

public class StoredPprocedureCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _allKeys = new();
    private readonly ILogger<StoredPprocedureCacheService> _logger;

    public StoredPprocedureCacheService(IMemoryCache cache, ILogger<StoredPprocedureCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is T cached)
        {
            _logger.LogInformation($"[CACHE HIT] key: {cacheKey}, result: {(cached == null ? "null" : (cached is ICollection col && col.Count == 0 ? "empty list" : "data available"))}");
            return cached;
        }

        var result = await factory();

        if (result != null)
        {
            _cache.Set(cacheKey, result, expiration ?? TimeSpan.FromMinutes(10));
            _logger.LogInformation($"[CACHE MISS] Key: {cacheKey} — Data loaded from DB and cached.");
            _allKeys.Add(cacheKey);
        }

        return result;
    }

    public void Remove(string cacheKey)
    {
        _cache.Remove(cacheKey);
        _allKeys.Remove(cacheKey);
    }

    public void RemoveByPrefix(string prefix)
    {
        var keysToRemove = _allKeys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _allKeys.Remove(key);
        }
    }
}
