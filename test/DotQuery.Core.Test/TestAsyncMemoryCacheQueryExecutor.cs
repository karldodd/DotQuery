using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Test.Stub;
using DotQuery.Extensions;
using Xunit;

namespace DotQuery.Core.Test
{
    public class TestAsyncMemoryCacheQueryExecutor
    {
        [Fact]
        public void TestDefaultKeySerializer()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };
            var q3 = new AddQuery { Left = 1, Right = 3 };

            var keySer = new DefaultKeySerializer<AddQuery>();
            Assert.Equal(keySer.SerializeToString(q1), keySer.SerializeToString(q2));
            Assert.NotEqual(keySer.SerializeToString(q1), keySer.SerializeToString(q3));
        }

        [Fact]
        public void TestQueryAsync()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            AddAsyncQueryExecutor executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestQueryAsyncWithOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestEmptyOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, EntryOptions.Empty, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, EntryOptions.Empty, CancellationToken.None)); })
                >=
                delayTime);
        }

        [Fact]
        public void TestDefaultOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, EntryOptions.Default, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, EntryOptions.Default, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestCustomOptions1()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions();

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestCustomOptions2()
        {
            var delayTime = TimeSpan.FromMilliseconds(200);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { SlidingExpiration = TimeSpan.FromMilliseconds(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Thread.Sleep(1);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);
        }

        [Fact]
        public void TestCustomOptions3()
        {
            var delayTime = TimeSpan.FromMilliseconds(200);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Thread.Sleep(1);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);
        }

        [Fact]
        public void TestCustomOptions4()
        {
            var delayTime = TimeSpan.FromMilliseconds(200);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Thread.Sleep(1);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);
        }

        [Fact]
        public void TestCustomOptions5()
        {
            var delayTime = TimeSpan.FromMilliseconds(200);
            var executor = CreateMemoryExecutor(delayTime);
            var q1 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue };

            //use cached failed task
            var q2 = new AddQuery { Left = int.MaxValue, Right = int.MaxValue };

            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await executor.QueryAsync(q1, CancellationToken.None); })));
            Assert.True(TryCatchException<OverflowException>(() => TimeCost(async () => { await executor.QueryAsync(q2, new EntryOptions { Behaviors = (EntryBehaviors)(EntryBehaviors.Default - EntryBehaviors.ReQueryWhenErrorCached) }, CancellationToken.None); })));

            Assert.Equal(1, executor.RealCalcCount);
        }

        [Fact]
        public void TestSlidingExpirationOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestAbsoluteExpirationRelativeToNowOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public void TestAbsoluteExpirationOptions()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            var options = new EntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) };

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                >=
                delayTime);

            Assert.True(
                TimeCost(async () => { Assert.Equal(3, await executor.QueryAsync(query, options, CancellationToken.None)); })
                <=
                TimeSpan.FromMilliseconds(50), "Should hit cache");  //well, a cache hit
        }

        [Fact]
        public async Task TestCancellationTokenAsync()
        {
            var delayTime = TimeSpan.FromMilliseconds(100);
            var executor = CreateMemoryExecutor(delayTime);

            var query = new AddQuery { Left = 1, Right = 2 };

            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(TimeSpan.FromMilliseconds(1));

                try
                {
                    await executor.QueryAsync(query, cts.Token);
                }
                catch (Exception ex)
                {
                    var ocex = ex as OperationCanceledException;

                    Assert.NotNull(ocex);
                }
            }
        }

        private static AddAsyncQueryExecutor CreateMemoryExecutor(TimeSpan delayTime)
        {
            return new AddAsyncQueryExecutor(new MemoryCacheBasedQueryCache<AddQuery, AsyncLazy<int>>(new DefaultKeySerializer<AddQuery>()), delayTime);
        }

        private TimeSpan TimeCost(Func<Task> a)
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
    }
}
