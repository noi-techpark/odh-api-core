// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace OdhApiCore
{
    public static class DistributedCachingExtensions
    {
        public static async Task SetCacheValueAsync<T>(
            this IDistributedCache distributedCache,
            string key,
            T value,
            CancellationToken token = default
        )
            where T : notnull
        {
            await distributedCache.SetAsync(key, value.ToByteArray(), token);
        }

        public static async Task SetCacheValueAsync<T>(
            this IDistributedCache distributedCache,
            string key,
            TimeSpan timewindow,
            T value,
            CancellationToken token = default
        )
            where T : notnull
        {
            //var options = new DistributedCacheEntryOptions {  AbsoluteExpiration = DateTimeOffset.Now.Add(timewindow) };
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timewindow,
            };
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);
        }

        public static async Task<T?> GetCacheValueAsync<T>(
            this IDistributedCache distributedCache,
            string key,
            CancellationToken token = default(CancellationToken)
        )
            where T : class
        {
            var result = await distributedCache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }
    }
}
