using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotQuery.Core.Async;
using DotQuery.Core.Caches;
using DotQuery.Core.Queries;
using Newtonsoft.Json;
using DotQuery.Core;
using DotQuery.Extensions.Http;

namespace DotQuery.Extensions.Http
{
    public abstract class WebApiQueryExecutorBase<TQuery, TResult> : AsyncQueryExecutor<TQuery, TResult> where TQuery : QueryBase
    {
        protected readonly WebApiClient m_webApiClient;
        protected readonly string m_apiPath;

        protected WebApiQueryExecutorBase(WebApiClient webApiClient, IQueryCache<TQuery, AsyncLazy<TResult>> queryCache, string apiPath) : base(queryCache)
        {
            m_webApiClient = webApiClient;
            m_apiPath = apiPath;
        }

        protected override async Task<TResult> DoQueryAsync(TQuery query)
        {
            var postBody = GetPostBody(query);

            HttpResponseMessage response;
            if (postBody != null)
            {
                response = await m_webApiClient.PostAsync(postBody, GetRequestPath(query));
                return await FromResponse(response);
            }
            else
            {
                return await m_webApiClient.GetAsync<TResult>(GetRequestPath(query));
            }
        }

        protected virtual object GetPostBody(TQuery query)
        {
            return new[] { query };
        }

        protected virtual string GetRequestPath(TQuery query)
        {
            return string.Format("{0}?exportBinary={1}", m_apiPath, query.ExportBinary);
        }

        protected virtual async Task<TResult> FromResponse(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<TResult>(await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync());
        }
    }
}
