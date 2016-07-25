using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}
