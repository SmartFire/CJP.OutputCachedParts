using System;
using CJP.OutputCachedParts.Extensions;
using CJP.OutputCachedParts.Filters;
using Orchard.Caching.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
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

            OnCreated<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedWidgetFilter.AllLayersCacheKey));
            OnUpdated<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedWidgetFilter.AllLayersCacheKey));
            OnRemoved<LayerPart>((ctx, part) => InvalidateLayerCaches(CachedWidgetFilter.AllLayersCacheKey));

            OnCreated<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
            OnUpdated<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
            OnRemoved<CommonPart>((ctx, part) => InvalidateContainerCaches(ctx));
        }

        private void InvalidateContainerCaches(ContentContextBase context)
        {
            var stereotype = context.ContentItem.GetStereotype();
            if (string.Equals(stereotype, "Widget", StringComparison.InvariantCultureIgnoreCase)) 
            {
                InvalidateLayerCaches(CachedWidgetFilter.WidgetContainersCacheKey);
            }
        }

        private void InvalidateLayerCaches(string cacheKey)
        {
            _cacheService.Remove(CachedWidgetFilter.PopulatedLayersCacheKey);
            _cacheService.Remove(cacheKey);
        }
    }
}