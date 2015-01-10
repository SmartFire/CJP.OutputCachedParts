using CJP.OutputCachedParts.Filters;
using Orchard.Caching.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Widgets.Models;

namespace CJP.OutputCachedParts.Handlers
{
    [OrchardFeature("CJP.OutputCachedParts.CachedWidgetFilter")]
    public class LayerPartCacheInvalidationHandler : ContentHandler
    {
        private readonly ICacheService _cacheService;

        public LayerPartCacheInvalidationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;

            OnUpdated<LayerPart>((ctx, part) => _cacheService.Remove(CachedWidgetFilter.AllLayersCacheKey));
        }
    }
}