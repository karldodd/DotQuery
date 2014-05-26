using System;
using System.Collections.Generic;
using System.Linq;

namespace DotQuery.Core.Queries
{
    /// <summary>
    /// A composite query that stands for a group of queries
    /// </summary>
    public class AggregateQuery : QueryBase
    {
        private readonly List<QueryBase> m_queries;

        /// <summary>
        /// Constructor of a composite query
        /// </summary>
        /// <param name="underlyingQueries">A group of underlying queries</param>
        /// <param name="export"></param>
        public AggregateQuery(IEnumerable<QueryBase> underlyingQueries, bool export = false) : base(export)
        {
            m_queries = underlyingQueries.ToList();
        }

        /// <summary>
        /// The underlying queries
        /// </summary>
        public IList<QueryBase> Queries
        {
            get { return m_queries; }
        }

        /// <summary>
        /// The event when one of the underlying queries is finished
        /// Notice: this can be called by a background thread so you should do the marshalling in listeners if necessary
        /// </summary>
        public event Action<QueryBase, object> SingleQueryFinished;

        internal virtual void OnSingleQueryFinished(QueryBase query, object result)
        {
            Action<QueryBase, object> handler = SingleQueryFinished;
            if (handler != null) handler(query, result);
        }
    }   
}
