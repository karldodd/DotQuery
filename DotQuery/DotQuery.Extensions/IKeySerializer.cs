using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotQuery.Extensions
{
    /// <summary>
    /// To support MemoryCacheBasedQueryCache
    /// </summary>
    public interface IKeySerializer<TKey>
    {
        string SerializeToString(TKey key);
    }

    public class DefaultKeySerializer<TKey> : IKeySerializer<TKey>
    {
        public string SerializeToString(TKey key)
        {
            return key.ToString();
        }
    }
}
