using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services {
    public interface ICacheKeyService : IDependency {
        string BuildFullCacheKey(ContentPart part, string cacheKey);
    }
}