using System;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace DotQuery.Core.Test
{
    public class TestEntryOptions
    {
        [Fact]
        public void TestEmptyOptions()
        {
            var options = EntryOptions.Empty;

            Assert.Equal(options.Behaviors, EntryBehaviors.None);

            Assert.Null(options.AbsoluteExpiration);
            Assert.Null(options.AbsoluteExpirationRelativeToNow);
            Assert.Null(options.SlidingExpiration);
        }

        [Fact]
        public void TestDefaultOptions()
        {
            var options = EntryOptions.Default;

            Assert.Equal(options.Behaviors, EntryBehaviors.Default);

            Assert.Null(options.AbsoluteExpiration);
            Assert.Null(options.AbsoluteExpirationRelativeToNow);
            Assert.Null(options.SlidingExpiration);

            Assert.True((options.Behaviors & EntryBehaviors.LookupCache) == EntryBehaviors.LookupCache);
            Assert.True((options.Behaviors & EntryBehaviors.SaveToCache) == EntryBehaviors.SaveToCache);
            Assert.True((options.Behaviors & EntryBehaviors.ReQueryWhenErrorCached) == EntryBehaviors.ReQueryWhenErrorCached);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options == EntryOptions.Default ? TimeSpan.FromMinutes(1) : options.SlidingExpiration
            };

            Assert.Equal(cacheOptions.SlidingExpiration, TimeSpan.FromMinutes(1));
            Assert.Null(cacheOptions.AbsoluteExpiration);
            Assert.Null(cacheOptions.AbsoluteExpirationRelativeToNow);
        }
    }
}
