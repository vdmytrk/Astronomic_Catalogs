namespace Astronomic_Catalogs.Services.Interfaces;

public interface ICacheService
{
    Task<T?> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;
    void Remove(string cacheKey);
    void RemoveByPrefix(string prefix);
}
