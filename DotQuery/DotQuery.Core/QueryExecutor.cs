using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    /// <summary>
    /// Core abstract class of this library that gracefully handles all cache read/hit/write on given query.
    /// All real query executor should inherit from this class and implement DoQueryAsync.
    /// </summary>
    /// <typeparam name="TQuery">Type of the query object</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public abstract class QueryExecutor<TQuery, TResult>
    {
        private readonly IQueryCache<TQuery> m_queryCache;

        /// <summary>
        /// Protected constructor of the query executor
        /// </summary>
        /// <param name="queryCache"></param>
        protected QueryExecutor(IQueryCache<TQuery> queryCache)
        {
            m_queryCache = queryCache;
        }

        /// <summary>
        /// Execute the given query as an asynchronous operation.
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task<TResult> QueryAsync(TQuery query)
        {
            var typedQuery = query as QueryBase;

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
        public async Task<TResult> QueryAsync(TQuery query, QueryOptions queryOptions)
        {
            object cachedValue;
            if (queryOptions.HasFlag(QueryOptions.LookupCache) && m_queryCache.TryGetFromCache(query, out cachedValue))
            {
                var queryTask = cachedValue as Task<TResult>;
                if (queryTask != null)
                {
                    //re-query if the task is canceld or failed.
                    if ((queryTask.IsFaulted || queryTask.IsCanceled) && queryOptions.HasFlag(QueryOptions.ReQueryWhenErrorCached))
                    {
                        return await QueryInternalAsync(query, queryOptions).ConfigureAwait(false);
                    }

                    //query is cached, just await the result
                    return await queryTask.ConfigureAwait(false);
                }

                if (cachedValue is TResult)
                {
                    var cachedStream = cachedValue as Stream;

                    //special treatment if the cached object is a stream
                    if (cachedStream != null)
                    {
                        if (cachedStream.CanSeek) //reusable, probably memory stream
                        {
                            cachedStream.Seek(0, SeekOrigin.Begin);
                            return (TResult)cachedValue;
                        }
                        else
                        {
                            //The stream is probably used... Dispose it and query again...
                            //todo: make cache stream configurable
                            cachedStream.Dispose();
                            return await QueryInternalAsync(query, queryOptions).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        //result is cached
                        return (TResult)cachedValue;
                    }
                }

                throw new QueryCacheException("An unintended result was cached in query cache: " + cachedValue);
            }
            else
            {
                return await QueryInternalAsync(query, queryOptions).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute the query and update the cache when necessary
        /// </summary>        
        protected async Task<TResult> QueryInternalAsync(TQuery query, QueryOptions queryOptions)
        {
            Task<TResult> task = DoQueryAsync(query);

            bool cacheResult = queryOptions.HasFlag(QueryOptions.SaveToCache);
            if (cacheResult) m_queryCache.CacheValue(query, task); //cache the task
            TResult result = await task.ConfigureAwait(false);
            if (cacheResult) m_queryCache.CacheValue(query, result); //cache the result
            return result;
        }

        /// <summary>
        /// Execute the query in the real world (without cache read/write)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract Task<TResult> DoQueryAsync(TQuery query);
    }

    /// <summary>
    /// Exception that representing invalid states inside query executor and its cache
    /// </summary>
    public class QueryCacheException : Exception
    {
        /// <summary>
        /// Constructs a QueryCacheException
        /// </summary>
        public QueryCacheException(string msg)
            : base(msg)
        {
        }
    }
}
