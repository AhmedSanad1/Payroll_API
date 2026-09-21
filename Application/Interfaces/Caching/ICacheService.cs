namespace PayRollApi.Application.Interfaces.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<string?> GetStringAsync(string key);
        Task SetStringAsync(string key, string value, TimeSpan? expiration = null);
    }
}