using System;
using System.Threading;
using DotQuery.Extensions;
using Xunit;

namespace DotQuery.Core.Test
{
    public class TestMemoryCacheBasedQueryCache
    {
        [Fact]
        public void TestTryGetOrAdd()
        {
            var cache = new MemoryCacheBasedQueryCache<string, int>(new DefaultKeySerializer<string>(), TimeSpan.FromSeconds(10));
            var val = cache.GetOrAdd("key", 1, new CacheEntryOptions { SlidingExpiration = TimeSpan.FromMilliseconds(1) });
            Assert.Equal(1, val);

            Thread.Sleep(5);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new CacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1) });
            Thread.Sleep(5);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new CacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(1) });
            Thread.Sleep(5);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new CacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(1) });
            Thread.Sleep(200);
            Assert.True(cache.TryGet("key", out val));
        }
    }
}
