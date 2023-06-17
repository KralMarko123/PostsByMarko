using Microsoft.Extensions.Caching.Distributed;

namespace PostsByMarko.Host.Extensions
{
    public static class DistributedCacheExtensions
    {
        public async static Task<T> GetCachedValueAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default) where T : class
        {
            var result = await cache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }

        public async static Task SetCachedValueAsync<T>(this IDistributedCache cache, string key, T value, CancellationToken token = default)
        {
            await cache.SetAsync(key, value.ToByteArray(), token);
        }
    }
}
