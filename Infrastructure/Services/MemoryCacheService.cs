using Microsoft.Extensions.Caching.Memory;
using PayRollApi.Application.Interfaces.Caching;

namespace PayRollApi.Infrastructure.Services
{
    public class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public Task<T?> GetAsync<T>(string key)
        {
            cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            cache.Set(key, value, expiration ?? DefaultExpiration);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (cache.TryGetValue(key, out T? cached) && cached is not null)
                return cached;

            var value = await factory();
            cache.Set(key, value, expiration ?? DefaultExpiration);
            return value;
        }

        public Task<string?> GetStringAsync(string key)
        {
            cache.TryGetValue(key, out string? value);
            return Task.FromResult(value);
        }

        public Task SetStringAsync(string key, string value, TimeSpan? expiration = null)
        {
            cache.Set(key, value, expiration ?? DefaultExpiration);
            return Task.CompletedTask;
        }
    }
}
