using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core.Test.Stub
{
    public class AddQueryExecutor : QueryExecutor<AddQuery, int>
    {
        private readonly TimeSpan m_delayTime;

        public AddQueryExecutor(TimeSpan delayTime) : base(new SimpleQueryCache(new DefaultQueryEqualityComparer()))
        {
            m_delayTime = delayTime;
        }

        protected override async Task<int> DoQueryAsync(AddQuery query)
        {
            await Task.Delay(m_delayTime);
            return query.Left + query.Right;
        }
    }
}
