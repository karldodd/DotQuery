using System;

namespace DotQuery.Core
{
    public class EntryOptions
    {
        /// <summary>
        /// The query does nothing related to the cache (raw execution every time)
        /// </summary>
        public static readonly EntryOptions Empty = new EntryOptions { Behaviors = EntryBehaviors.None };

        /// <summary>
        /// The default smart behavior: the query will lookup and save to cache, and re-execute the query if error is cached
        /// </summary>
        public static readonly EntryOptions Default = new EntryOptions { Behaviors = EntryBehaviors.Default };

        private DateTimeOffset? _absoluteExpiration;
        private TimeSpan? _absoluteExpirationRelativeToNow;
        private TimeSpan? _slidingExpiration;
        private EntryBehaviors _entryBehaviors;

        /// <summary>
        /// Gets or sets the query behavior for the cache entry.
        /// </summary>
        public EntryBehaviors Behaviors { get; set; } = EntryBehaviors.Default;

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration
        {
            get
            {
                return _absoluteExpiration;
            }
            set
            {
                _absoluteExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get
            {
                return _absoluteExpirationRelativeToNow;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive.");
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get
            {
                return _slidingExpiration;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive.");
                }
                _slidingExpiration = value;
            }
        }
    }
}