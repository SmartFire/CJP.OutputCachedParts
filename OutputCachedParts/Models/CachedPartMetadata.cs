using System;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.OutputCachedParts.Models 
{
    public class CachedPartMetadata
    {
        public CachedPartMetadata(string cacheKey)
        {
            CacheKey = cacheKey;
        }
        public CachedPartMetadata(string cacheKey, TimeSpan timespan)
        {
            CacheKey = cacheKey;
            Timespan = timespan;
        }

        public string CacheKey { get; set; }
        public TimeSpan? Timespan { get; set; }
    }
}