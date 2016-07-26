using System.Threading.Tasks;

namespace DotQuery.Core
{
    public interface IAsyncQueryExecutor<TQuery, TResult>
    {
        Task<TResult> QueryAsync(TQuery query);
    }
}
