using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;
using DotQuery.Core.Queries;

namespace DotQuery.Core.Test.Stub
{
    public class AddAsyncQueryExecutor : AsyncQueryExecutor<AddQuery, int>
    {
        private readonly TimeSpan m_delayTime;

        public AddAsyncQueryExecutor(IQueryCache<AddQuery, AsyncLazy<int>> cache, TimeSpan delayTime) : base(cache)
        {
            m_delayTime = delayTime;
        }

        public AddAsyncQueryExecutor(TimeSpan delayTime)
            : this(new SimpleQueryCache<AddQuery, AsyncLazy<int>>(new DefaultQueryEqualityComparer()), delayTime)
        {
        }

        protected override async Task<int> DoQueryAsync(AddQuery query)
        {
            RealCalcCount++;
            await Task.Delay(m_delayTime);
            checked
            {
                return query.Left + query.Right;
            }
        }

        public int RealCalcCount { get; set; }
    }
}
