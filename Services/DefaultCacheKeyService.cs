using System.Collections.Generic;
using System.Linq;
using CJP.OutputCachedParts.Providers;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services
{
    public class DefaultCacheKeyService : ICacheKeyService {
        private readonly IEnumerable<ICacheKeyCompositeProvider> _cacheKeyCompositeProviders;

        public DefaultCacheKeyService(IEnumerable<ICacheKeyCompositeProvider> cacheKeyCompositeProviders) {
            _cacheKeyCompositeProviders = cacheKeyCompositeProviders;
        }

        public string BuildFullCacheKey(ContentPart part, string cacheKey) {
            var suffixes = _cacheKeyCompositeProviders.Select(p => p.GetCompositeValue()).OrderBy(v => v);

            return string.Format("CJP.OutputCachedWidgets-{0}-{1}-{2}", part.Id, string.Join("-", suffixes), cacheKey);
        }
    }
}