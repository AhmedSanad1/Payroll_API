using PayRollApi.Application.Common;

namespace PayRollApi.Application.Interfaces.Caching
{
    // MemoryCache can't remove keys by pattern, so every paged key carries a version stamp.
    // A write just bumps the stamp and all the old pages become unreachable.
    public static class CacheServiceExtensions
    {
        private static readonly TimeSpan DefaultPagedExpiration = TimeSpan.FromSeconds(60);

        public static async Task<TResult> GetOrCreatePagedAsync<TEntity, TResult>(
            this ICacheService cache, string variantKey, Func<Task<TResult>> factory, TimeSpan? expiration = null)
        {
            var version = await cache.GetOrCreateAsync(
                CacheKeys.PagedListVersion<TEntity>(), () => Task.FromResult(Guid.NewGuid().ToString("N")));

            var key = $"{CacheKeys.EntityList<TEntity>()}:paged:{version}:{variantKey}";
            return await cache.GetOrCreateAsync(key, factory, expiration ?? DefaultPagedExpiration);
        }

        public static Task InvalidatePagedAsync<TEntity>(this ICacheService cache) =>
            cache.RemoveAsync(CacheKeys.PagedListVersion<TEntity>());
    }
}
