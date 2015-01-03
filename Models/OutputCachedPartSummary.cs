using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Models {
    public class OutputCachedPartSummary
    {
        public int Id { get; set; }
        public string CacheKey { get; set; }
        public string PartName { get; set; }
        public string ContentType { get; set; }
        public string ContentName { get; set; }
        public int ContentId { get; set; }
        public ContentItem Content { get; set; }
        public OutputCachedPartsModel CachedValue { get; set; }
    }
}