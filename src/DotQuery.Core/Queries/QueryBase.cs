using System;
using Newtonsoft.Json;

namespace DotQuery.Core.Queries
{
    /// <summary>
    /// The base class for any query object
    /// </summary>
    public abstract class QueryBase
    {
        protected QueryBase(bool exportQuery = false)
        {
            this.ExportBinary = exportQuery;
            this.CreateTime = DateTime.Now;
            //this.QueryOptions = EntryBehaviors.Default;
        }

        public bool ExportBinary { get; protected set; }

        [Obsolete("Deprecated!", true)]
        [JsonIgnore]
        public EntryBehaviors QueryOptions { get; set; }

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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
