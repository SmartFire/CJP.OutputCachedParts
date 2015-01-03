using System;
using System.Collections.Generic;
using System.Web;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using CJP.OutputCachedParts.Services;
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
        private readonly IOutputCachedPartsService _outputCachedPartsService;

        public OutputCachedPartsDisplayManager(IWorkContextAccessor workContextAccessor, IEnumerable<IShapeDisplayEvents> shapeDisplayEvents, Lazy<IShapeTableLocator> shapeTableLocator, IOutputCachedPartsService outputCachedPartsService) 
            : base(workContextAccessor, shapeDisplayEvents, shapeTableLocator) {
            _outputCachedPartsService = outputCachedPartsService;
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

            return _outputCachedPartsService.BuildAndCacheOutput(() => base.Execute(context), part);
        }
    }
}