using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core.Test.Stub
{
    public class AddAsyncQueryExecutor : AsyncQueryExecutor<AddQuery, int>
    {
        private readonly TimeSpan m_delayTime;

        public AddAsyncQueryExecutor(TimeSpan delayTime) : base(new SimpleQueryCache<AddQuery, int>(new DefaultQueryEqualityComparer()))
        {
            m_delayTime = delayTime;
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
