using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore
{
    public static class DistributedCachingExtensions
    {
        public async static Task SetCacheValueAsync<T>(this IDistributedCache distributedCache, string key, T value, CancellationToken token = default) where T : notnull
        {
            await distributedCache.SetAsync(key, value.ToByteArray(), token);
        }

        public async static Task SetCacheValueAsync<T>(this IDistributedCache distributedCache, string key, TimeSpan timewindow, T value, CancellationToken token = default) where T : notnull
        {
            //var options = new DistributedCacheEntryOptions {  AbsoluteExpiration = DateTimeOffset.Now.Add(timewindow) };
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timewindow };
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);
        }

        public async static Task<T?> GetCacheValueAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);

            if(result != null)
            {

            }

            return result.FromByteArray<T>();
        }
    }
}
