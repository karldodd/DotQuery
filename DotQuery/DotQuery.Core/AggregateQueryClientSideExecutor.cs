﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    /// <summary>
    /// The query executor which is able to fire a group of queries as a single composite query.
    /// You can get back a list of results later.
    /// </summary>
    /// <remarks>
    /// This class requires all child queries/results to be in the same type/shape
    /// </remarks>
    /// <typeparam name="TQuery">The type of a child query</typeparam>
    /// <typeparam name="TResult">The result type of a child query</typeparam>
    public class AggregateQueryClientSideExecutor<TQuery, TResult> : QueryExecutor<AggregateQuery, List<TResult>> 
    {
        private readonly QueryExecutor<TQuery, TResult> m_childQueryExecutor;

        /// <summary>
        /// Constructs an AggregateQueryClientSideExecutor
        /// </summary>
        /// <param name="childQueryExecutor">The query executor to execute child queries</param>
        /// <param name="queryCache">The query cache to be used</param>
        public AggregateQueryClientSideExecutor(QueryExecutor<TQuery, TResult> childQueryExecutor, IQueryCache<AggregateQuery> queryCache)
            : base(queryCache)
        {
            m_childQueryExecutor = childQueryExecutor;
        }

        protected override Task<List<TResult>> DoQueryAsync(AggregateQuery aq)
        {
            if (aq.ExportBinary)
            {
                //that should be done at server side
                throw new NotSupportedException("AggregateQueryClientSideExecutor only handles client-side query aggreation");
            }

            List<Task<TResult>> taskList = aq.Queries.Cast<TQuery>().Select(childQ => m_childQueryExecutor.QueryAsync(childQ, aq.QueryOptions)).ToList(); //query all queries inside the Aggregated Query
            var queryList = aq.Queries.ToList();

            //Could use Task.WhenAll() instead if we don't want 'OnSingleQueryFinished' event
            //Could use Task.Run() here but for .net4 compatibility we go for Task.Factory.StartNew(async).Unwrap(). For more information, see http://blogs.msdn.com/b/pfxteam/archive/2011/10/24/10229468.aspx
            Task<List<TResult>> taskParent = Task.Factory.StartNew(async () =>
            {
                var results = new List<TResult>();

                //populate some slots in the list
                for (int i = 0; i < queryList.Count; i++)
                {
                    results.Add(default(TResult));
                }

                //wait all one by one pattern
                while (taskList.Count > 0)
                {
#if portable
                    Task<TResult> completedTask = await TaskEx.WhenAny(taskList);                    
#else
                    Task<TResult> completedTask = await Task.WhenAny(taskList);
#endif
                    int index = taskList.IndexOf(completedTask);
                    QueryBase query = queryList[index];

                    if (completedTask.Exception == null)
                    {
                        TResult qr = completedTask.Result;
                        aq.OnSingleQueryFinished(query, qr);
                        results[aq.Queries.IndexOf(query)] = qr; //put result into the correct postion (todo: ensure indexof is using reference equal there )
                    }
                    else
                    {
                        Exception exp = completedTask.Exception.InnerExceptions.FirstOrDefault(); //unwrap aggregate exception

                        if (exp != null)
                        {
                            throw exp;
                        }
                    }

                    taskList.RemoveAt(index);
                    queryList.RemoveAt(index);
                }

                return results;
            }).Unwrap();

            return taskParent;
        }
    }
}
