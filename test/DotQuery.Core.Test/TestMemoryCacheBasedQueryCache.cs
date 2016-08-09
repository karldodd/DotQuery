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
            var cache = new MemoryCacheBasedQueryCache<string, int>(new DefaultKeySerializer<string>());
            var val = cache.GetOrAdd("key", 1, new EntryOptions { SlidingExpiration = TimeSpan.FromMilliseconds(1) });
            Assert.Equal(1, val);

            Thread.Sleep(1);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new EntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1) });
            Thread.Sleep(1);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new EntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(1) });
            Thread.Sleep(1);
            Assert.False(cache.TryGet("key", out val));

            cache.Set("key", 1, new EntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(1) });
            Thread.Sleep(100);
            Assert.True(cache.TryGet("key", out val));
        }

        [Fact]
        public void TestRemove()
        {
            var cache = new MemoryCacheBasedQueryCache<string, int>(new DefaultKeySerializer<string>());

            string testKey = "testKey";
            int testValue = 1;
            cache.Set(testKey, testValue, EntryOptions.Default);

            int value;
            Assert.True(cache.TryGet(testKey, out value));
            Assert.Equal(testValue, value);

            cache.Remove(testKey);

            Assert.False(cache.TryGet(testKey, out testValue));
        }
    }
}
