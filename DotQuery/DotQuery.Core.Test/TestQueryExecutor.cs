using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotQuery.Core.Test.Stub;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DotQuery.Core.Test
{
    [TestClass]
    public class TestQueryExecutor
    {
        private TimeSpan m_delayTime;
        private QueryExecutor<AddQuery, int> m_exec;

        private static TimeSpan TimeCost(Func<Task> a)
        {
            Stopwatch sw = Stopwatch.StartNew();
            a().Wait();
            return sw.Elapsed;
        }

        [TestInitialize]
        public void Init()
        {
            m_delayTime = TimeSpan.FromSeconds(1);
            m_exec = new AddQueryExecutor(m_delayTime);
        }

        [TestMethod]
        public void TestCacheHit()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 };

            Assert.IsTrue(
                TimeCost(async () => { Assert.AreEqual(3, await m_exec.QueryAsync(q1)); }) 
                >= 
                m_delayTime);

            Assert.IsTrue(
                TimeCost(async () => { Assert.AreEqual(3, await m_exec.QueryAsync(q2)); })
                <=
                TimeSpan.FromMilliseconds(10));  //well, a cache hit
        }

        [TestMethod]
        public void TestQueryOptions()
        {
            var q1 = new AddQuery { Left = 1, Right = 2 };
            var q2 = new AddQuery { Left = 1, Right = 2 , QueryOptions = QueryOptions.None};

            Assert.IsTrue(
                TimeCost(async () => { Assert.AreEqual(3, await m_exec.QueryAsync(q1)); })
                >=
                m_delayTime);

            Assert.IsTrue(
                TimeCost(async () => { Assert.AreEqual(3, await m_exec.QueryAsync(q2)); })
                >=
                m_delayTime);  //well, should not hit
        }
    }
}
