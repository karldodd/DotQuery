using System;
using DotQuery.Core.Async;

namespace DotQuery.Core
{
    /// <summary>
    /// Core abstract class of this library that gracefully handles all cache read/hit/write on given query.
    /// All real sync query executor should inherit from this class and implement DoQuerySync.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query object</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public abstract class SyncQueryExecutor<TQuery, TResult>
    {
        private readonly IQueryCache<TQuery, Lazy<TResult>> m_queryTaskCache;

        /// <summary>
        /// Protected constructor of the query executor
        /// </summary>
        /// <param name="queryCache"></param>
        protected SyncQueryExecutor(IQueryCache<TQuery, Lazy<TResult>> queryCache)
        {
            m_queryTaskCache = queryCache;
        }

        /// <summary>
        /// Execute the given query as an synchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>The result of the given query</returns>
        public TResult QuerySync(TQuery query)
        {
            var typedQuery = query as QueryBase; //todo: remove this

            if (typedQuery != null)
            {
                return this.QuerySync(query, typedQuery.QueryOptions);
            }

            return this.QuerySync(query, QueryOptions.Default);
        }

        /// <summary>
        /// Execute the given query as an synchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="queryOptions">Query options to use for this query</param>
        /// <returns>The result of the given query</returns>
        public TResult QuerySync(TQuery query, QueryOptions queryOptions)
        {
            if (queryOptions.HasFlag(QueryOptions.LookupCache))
            {
                if (queryOptions.HasFlag(QueryOptions.SaveToCache))
                {
                    return m_queryTaskCache.GetOrAdd(query, new Lazy<TResult>(() => DoQuerySync(query))).Value;
                }
                else
                {
                    Lazy<TResult> lazyResult;
                    if (m_queryTaskCache.TryGet(query, out lazyResult))
                    {
                        return lazyResult.Value;
                    }
                    else
                    {
                        return DoQuerySync(query);
                    }
                }
            }
            else
            {
                if (queryOptions.HasFlag(QueryOptions.SaveToCache))
                {
                    var newQueryTask = new Lazy<TResult>(() => DoQuerySync(query));
                    m_queryTaskCache.Set(query, newQueryTask);
                    return newQueryTask.Value;
                }
                else
                {
                    return DoQuerySync(query);
                }
            }
        }

        /// <summary>
        /// Execute the query in the real world (without cache read/write)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract TResult DoQuerySync(TQuery query);
    }
}
