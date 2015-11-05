using System;
using CJP.OutputCachedParts.Extensions;
using CJP.OutputCachedParts.Services;
using Orchard.Caching.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Widgets.Models;

namespace CJP.OutputCachedParts.Handlers
{
    [OrchardFeature("CJP.OutputCachedParts.CachedLayers")]
    public class LayerPartCacheInvalidationHandler : ContentHandler
    {
        private readonly ICacheService _cacheService;

        public LayerPartCacheInvalidationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;

            OnCreated<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedLayerRetrievalService.AllLayersCacheKey));
            OnUpdated<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedLayerRetrievalService.AllLayersCacheKey));
            OnRemoved<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedLayerRetrievalService.AllLayersCacheKey));

            OnCreated<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
            OnUpdated<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
            OnRemoved<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
        }

        private void InvalidateContainerCaches(ContentContextBase context)
        {
            var stereotype = context.ContentItem.GetStereotype();
            if (string.Equals(stereotype, "Widget", StringComparison.InvariantCultureIgnoreCase)) 
            {
                InvalidateLayerCaches(CachedLayerRetrievalService.WidgetContainersCacheKey);
            }
        }

        private void InvalidateLayerCaches(string cacheKey)
        {
            _cacheService.Remove(CachedLayerRetrievalService.PopulatedLayersCacheKey);
            _cacheService.Remove(cacheKey);
        }
    }
}