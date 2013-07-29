using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using DotQuery.Core;

namespace DotQuery.Core
{
    public abstract class QueryBase : CacheKey
    {
        protected QueryBase(int profileId, bool exportQuery = false)
        {
            this.ProfileId = profileId;
            this.ExportBinary = exportQuery;
            this.CreateTime = DateTime.Now;
            this.QueryOptions = QueryOptions.Default;
        }

        public int ProfileId { get; private set; }
        public bool ExportBinary { get; protected set; }

        public QueryOptions QueryOptions { get; set; }

        /// <summary>
        /// Should not involve this property in Hash/Equal stuff because it will break query result caching
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Majorly used for sheet naming in excel.
        /// You can also use it for:
        /// (1) multiple query distinction (for aggregate query)
        /// (2) explicit difference between two queries
        /// </summary>
        public string DisplayName { get; set; }
    }
}
