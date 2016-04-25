using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;
using DotQuery.Core.Queries;

namespace DotQuery.Core
{
    /// <summary>
    /// Core abstract class of this library that gracefully handles all cache read/hit/write on given query.
    /// All real query executor should inherit from this class and implement DoQueryAsync.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query object</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public abstract class AsyncQueryExecutor<TQuery, TResult>
    {
        private readonly IQueryCache<TQuery, AsyncLazy<TResult>> m_queryTaskCache;

        /// <summary>
        /// Protected constructor of the query executor
        /// </summary>
        /// <param name="queryCache"></param>
        protected AsyncQueryExecutor(IQueryCache<TQuery, AsyncLazy<TResult>> queryCache)
        {
            m_queryTaskCache = queryCache;
        }

        /// <summary>
        /// Execute the given query as an asynchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task<TResult> QueryAsync(TQuery query)
        {
            var typedQuery = query as QueryBase; //todo: remove this

            if (typedQuery != null)
            {
                return this.QueryAsync(query, typedQuery.QueryOptions);
            }

            return this.QueryAsync(query, QueryOptions.Default);
        }

        /// <summary>
        /// Execute the given query as an asynchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="queryOptions">Query options to use for this query</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task<TResult> QueryAsync(TQuery query, QueryOptions queryOptions)
        {
            Task<TResult> queryTask = null;
            if ((queryOptions & QueryOptions.LookupCache) == QueryOptions.LookupCache)
            {
                if ((queryOptions & QueryOptions.SaveToCache) == QueryOptions.SaveToCache)
                {
                    queryTask = m_queryTaskCache.GetOrAdd(query, new AsyncLazy<TResult>(() => DoQueryAsync(query))).Value;
                }
                else
                {
                    AsyncLazy<TResult> asyncLazyResult;
                    if (m_queryTaskCache.TryGet(query, out asyncLazyResult))
                    {
                        queryTask = asyncLazyResult.Value;                        
                    }
                    else {
                        //no task cached, do it directly
                        return DoQueryAsync(query);
                    }
                }
                
                //re-query if the task is canceld or failed.
                if ((queryTask.IsFaulted || queryTask.IsCanceled) && (queryOptions & QueryOptions.ReQueryWhenErrorCached) == QueryOptions.ReQueryWhenErrorCached)
                {
                    this.QueryAsync(query, queryOptions ^ QueryOptions.LookupCache);
                }

                //query is cached, just await the result
                return queryTask;                
            }
            else
            {
                if ((queryOptions & QueryOptions.SaveToCache) == QueryOptions.SaveToCache)
                {
                    var newQueryTask = new AsyncLazy<TResult>(() => DoQueryAsync(query));
                    m_queryTaskCache.Set(query, newQueryTask);
                    return newQueryTask.Value;
                }
                else
                {
                    return DoQueryAsync(query);
                }
            }
        }
        
        /// <summary>
        /// Execute the query in the real world (without cache read/write)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract Task<TResult> DoQueryAsync(TQuery query);
    }
}
