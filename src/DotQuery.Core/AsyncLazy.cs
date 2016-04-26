using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DotQuery.Core.Async
{
    //http://blogs.msdn.com/b/pfxteam/archive/2011/01/15/10116210.aspx
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory)) { }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }        
    }
}
