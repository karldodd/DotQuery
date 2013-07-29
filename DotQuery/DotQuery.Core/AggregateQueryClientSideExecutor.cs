using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Core
{
    public class AggregateQueryClientSideExecutor<TQuery, TResult> : QueryExecutor<AggregateQuery, List<TResult>> where TQuery : QueryBase
    {
        private readonly QueryExecutor<TQuery, TResult> m_dataProvider;

        public AggregateQueryClientSideExecutor(QueryExecutor<TQuery, TResult> dataProvider, IQueryCache queryCache)
            : base(queryCache)
        {
            m_dataProvider = dataProvider;
        }

        protected override Task<List<TResult>> DoQueryAsync(AggregateQuery aq)
        {
            if (aq.ExportBinary)
            {
                //that should be done at server side
                throw new NotSupportedException("This class only handles client-side query aggreation");
            }

            List<Task<TResult>> taskList = aq.Queries.Cast<TQuery>().Select(m_dataProvider.QueryAsync).ToList(); //query all queries inside the Aggregated Query
            var queryList = aq.Queries.ToList();

            //We could use Task.WhenAll() instead if we don't want 'OnSingleQueryFinished' event
            Task<List<TResult>> taskParent = Task.Run(async () =>
            {
                var results = new List<TResult>();

                //populate some slots in the list
                for (int i = 0; i < queryList.Count; i++)
                {
                    results.Add(default(TResult));
                }

                bool hasError = false;

                //wait all one by one pattern
                while (taskList.Count > 0)
                {
                    Task<TResult> completedTask = await Task.WhenAny(taskList);
                    int index = taskList.IndexOf(completedTask);
                    var query = queryList[index];

                    if (completedTask.Exception == null)
                    {
                        TResult qr = completedTask.Result;
                        aq.OnSingleQueryFinished(qr);
                        results[aq.Queries.IndexOf(query)] = qr; //put result into the correct postion (todo: ensure indexof is using reference equal there )
                    }
                    else
                    {
                        hasError = true;
                        Exception exp = completedTask.Exception.InnerExceptions.FirstOrDefault(); //unwrap aggregate exception
                        throw exp;
                    }

                    taskList.RemoveAt(index);
                    queryList.RemoveAt(index);
                }

                return results;
            });

            return taskParent;
        }
    }
}
