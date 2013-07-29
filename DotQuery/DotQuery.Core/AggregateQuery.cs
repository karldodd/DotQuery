using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public class AggregateQuery : QueryBase
    {
        private readonly List<QueryBase> m_queries;

        public AggregateQuery(IEnumerable<QueryBase> underlyingQueries, bool export = false)
            : base(-1, export)
        {
            m_queries = underlyingQueries.ToList();
        }

        public IList<QueryBase> Queries
        {
            get { return m_queries; }
        }

        /// <summary>
        /// Notice: this can be called by a background thread so you should do the marshalling in listeners if necessary
        /// </summary>
        public event Action<object> SingleQueryFinished;

        public virtual void OnSingleQueryFinished(object result)
        {
            Action<object> handler = SingleQueryFinished;
            if (handler != null) handler(result);
        }
    }

    public class AggregateQueryException : Exception
    {
        public List<Exception> Exceptions { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Exceptions.Count; i++)
            {
                sb.Append("(" + i + ")" + Exceptions[i].ToString() + "\n");
            }
            return base.ToString() + "\nInner Exceptions:\n" + sb.ToString();
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();

                for (int i = 0; i < Exceptions.Count; i++)
                {
                    sb.Append("(" + i + ")" + Exceptions[i].Message + "\n");
                }

                return "Oooops, query failed..." + "\nInner Messages:\n" + sb;
            }
        }
    }
}
