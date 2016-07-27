using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;
using DotQuery.Core.Test.Stub;
using DotQuery.Extensions;
using Xunit;

namespace DotQuery.Core.Test
{
    public class TestQueryExecutor
    {
        private TimeSpan m_delayTime;
        private AsyncQueryExecutor<AddQuery, int> m_exec;

        /// <summary>
        /// Save result/task to the cache when the query has been executed or is being executed
        /// </summary>
        private readonly EntryOptions SaveToCache = new EntryOptions { Behaviors = EntryBehaviors.SaveToCache, SlidingExpiration = TimeSpan.FromMinutes(1) };

        private static TimeSpan TimeCost(Func<Task> a)
        {
            Stopwatch sw = Stopwatch.StartNew();
            a().Wait();
            return sw.Elapsed;
        }

        private static bool TryCatchException<TException>(Action a) where TException : Exception
        {
            try
            {
                a();
            }
            catch (AggregateException e)
            {
                if (e.InnerException is TException)
                {
                    return true;
                }
            }
            catch (TException)
            {
                return true;
            }
            return false;
        }

        public TestQueryExecutor()
        {
            m_delayTime = TimeSpan.FromMilliseconds(100);
            m_exec = new AddAsyncQueryExecutor(m_delayTime);
        }


        [Fact]
        public void TestDefaultKeySerializer()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            var keySer = new DefaultKeySerializer<AddQuery>();
            Assert.Equal(keySer.SerializeToString(q1), keySer.SerializeToString(q2));
        }

        [Fact]
        public void TestCacheHit()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q2)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestQueryOptions()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q2, EntryOptions.Empty)); })
                >=
                m_delayTime);  //well, should not hit
        }

        [Fact]
        public void TestQueryOptions2()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q2, SaveToCache)); })
                >=
                m_delayTime);  //well, should not hit
        }

        [Fact]
        public void TestQueryOptions3()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1, SaveToCache)); })
                >=
                m_delayTime, "Should take longer");

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q2)); })
                <=
                TimeSpan.FromMilliseconds(10), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestQueryOptions4()
        {
            var q1 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue };
            var q2 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue };

            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await m_exec.QueryAsync(q1); })));
            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await m_exec.QueryAsync(q2); })));

            Assert.Equal(2, ((AddAsyncQueryExecutor)m_exec).RealCalcCount);
        }

        [Fact]
        public void TestQueryOptions5()
        {
            var q1 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue };

            //use cached failed task
            var q2 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue }; //, Options = new EntryOptions { Behaviors = (EntryBehaviors)(EntryBehaviors.Default - EntryBehaviors.ReQueryWhenErrorCached) } };

            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await m_exec.QueryAsync(q1); })));
            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await m_exec.QueryAsync(q2, new EntryOptions { Behaviors = (EntryBehaviors)(EntryBehaviors.Default - EntryBehaviors.ReQueryWhenErrorCached) }); })));

            Assert.Equal(1, ((AddAsyncQueryExecutor)m_exec).RealCalcCount);
        }

        [Fact]
        public void TestMemoryCacheQueryCache()
        {
            m_exec = new AddAsyncQueryExecutor(new MemoryCacheBasedQueryCache<AddQuery, AsyncLazy<int>>(new DefaultKeySerializer<AddQuery>(), TimeSpan.FromMinutes(1)), m_delayTime);
            var q1 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime, "Should take longer");

            Assert.True(    //TODO Test failure.
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestEmptyQueryCache()
        {
            m_exec = new AddAsyncQueryExecutor(EmptyQueryCache<AddQuery, AsyncLazy<int>>.Instance, m_delayTime);
            var q1 = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime, "Should take longer");

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime, "Should take longer");
        }

        [Fact]
        public void TestCacheEntryOptions()
        {
            var cache = new MemoryCacheBasedQueryCache<AddQuery, AsyncLazy<int>>(new DefaultKeySerializer<AddQuery>(), TimeSpan.FromMinutes(1));
            m_exec = new AddAsyncQueryExecutor(cache, m_delayTime);
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var options = new EntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) };

            Assert.Equal(EntryBehaviors.Default, options.Behaviors);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1, options)); })
                >=
                m_delayTime, "Should take longer");

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await m_exec.QueryAsync(q1, options)); }) //TODO
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }
    }
}
