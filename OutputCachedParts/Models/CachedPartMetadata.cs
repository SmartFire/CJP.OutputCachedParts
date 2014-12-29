using System;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.OutputCachedParts.Models 
{
    public class CachedPartMetadata
    {
        public CachedPartMetadata(ContentPart part, string cacheKey)
        {
            CacheKey = BuildCacheKey(part, cacheKey);
        }
        public CachedPartMetadata(ContentPart part, string cacheKey, TimeSpan timespan)
        {
            CacheKey = BuildCacheKey(part, cacheKey);
            Timespan = timespan;
        }

        public string CacheKey { get; set; }
        public TimeSpan? Timespan { get; set; }

        private string BuildCacheKey(ContentPart part, string cacheKey) {
            return string.Format("CJP.OutputCachedWidgets-{0}-{1}", cacheKey, part.Id);
        }
    }
}