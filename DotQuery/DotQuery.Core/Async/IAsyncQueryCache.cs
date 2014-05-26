namespace DotQuery.Core.Async
{
    public interface IAsyncQueryCache<in TKey, TValue>
    {
        AsyncLazy<TValue> GetOrAdd(TKey key, AsyncLazy<TValue> lazyTask);
        bool TryGet(TKey key, out AsyncLazy<TValue> value);
        void Set(TKey key, AsyncLazy<TValue> value);
        void Trim();
        void Clear();
    }
}
