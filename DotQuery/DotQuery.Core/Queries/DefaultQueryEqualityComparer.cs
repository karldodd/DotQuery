using System.Collections.Generic;

namespace DotQuery.Core.Queries
{
    /// <summary>
    /// To override default GetHashCode and Equals
    /// </summary>
    public class DefaultQueryEqualityComparer : IEqualityComparer<QueryBase>
    {
        public bool Equals(QueryBase x, QueryBase y)
        {
            return object.ReferenceEquals(x, y) || x.ToString() == y.ToString();
        }

        public int GetHashCode(QueryBase obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
