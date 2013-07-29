using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    /// <summary>
    /// To override default GetHashCode and Equals
    /// </summary>
    public class DefaultQueryEqualityComparer : IEqualityComparer<CacheKey>
    {
        public bool Equals(CacheKey x, CacheKey y)
        {
            return x.StringKey == y.StringKey;
        }

        public int GetHashCode(CacheKey obj)
        {
            return obj.StringKey.GetHashCode();
        }
    }
}
