using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotQuery.Core
{
    public abstract class CacheKey
    {
        [JsonIgnore]
        protected string m_stringKey;

        [JsonIgnore]
        public string StringKey
        {
            get
            {
                if (m_stringKey == null)
                {
                    //todo: make key generator configurable (core lib doesn't depend on Json.Net?)
                    m_stringKey = JsonConvert.SerializeObject(this); //use json as the key
                }

                return m_stringKey;
            }
        }
    }
}
