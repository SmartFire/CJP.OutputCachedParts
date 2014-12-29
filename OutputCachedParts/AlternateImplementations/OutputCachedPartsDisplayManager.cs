using System;
using System.Collections.Generic;
using System.Web;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;

namespace CJP.OutputCachedParts.OutputCachedParts.AlternateImplementations
{
    [OrchardSuppressDependency("Orchard.DisplayManagement.Implementation.DefaultDisplayManager")]
    public class OutputCachedPartsDisplayManager : DefaultDisplayManager, IDisplayManager
    {
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;

        public OutputCachedPartsDisplayManager(IWorkContextAccessor workContextAccessor, IEnumerable<IShapeDisplayEvents> shapeDisplayEvents, Lazy<IShapeTableLocator> shapeTableLocator, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext) 
            : base(workContextAccessor, shapeDisplayEvents, shapeTableLocator) {
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
        }

        public new IHtmlString Execute(DisplayContext context)
        {
            ContentPart part;
            var shape = ((dynamic)context.Value);

            try
            {
                part = (ContentPart)shape.ContentPart;
            }
            catch (Exception ex)
            {
                //this means that this is not a content part being displayed
                return base.Execute(context);
            }

            if (part == null)
            {
                return base.Execute(context);
            }

            var cachedPartMetadata = _outputCachedPartsContext.GetCachedPartMetadata(part);

            if (cachedPartMetadata == null)
            {
                return base.Execute(context);
            }


            var cachedModel = _outputCachedPartsContext.GetCacheModel(() => base.Execute(context).ToHtmlString());

            if (cachedPartMetadata.Timespan.HasValue)
            {
                _cacheService.Put(cachedPartMetadata.CacheKey, cachedModel, cachedPartMetadata.Timespan.Value);
            }
            else
            {
                _cacheService.Put(cachedPartMetadata.CacheKey, cachedModel);
            }

            return new HtmlString(cachedModel.Html);
        }
    }
}