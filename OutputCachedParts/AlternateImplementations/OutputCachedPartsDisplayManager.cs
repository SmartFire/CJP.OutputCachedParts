using System;
using System.Collections.Generic;
using System.Web;
using CJP.OutputCachedParts.Services;
using Glimpse.Orchard.AlternateImplementations;
using Glimpse.Orchard.PerformanceMonitors;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;

namespace CJP.OutputCachedParts.OutputCachedParts.AlternateImplementations
{
    [OrchardSuppressDependency("Orchard.DisplayManagement.Implementation.DefaultDisplayManager")]
    [OrchardSuppressDependency("Glimpse.Orchard.AlternateImplementations.GlimpseDisplayManager")]
    public class OutputCachedPartsDisplayManager : GlimpseDisplayManager, IDisplayManager
    {
        private readonly IOutputCachedPartsService _outputCachedPartsService;


        public OutputCachedPartsDisplayManager(IWorkContextAccessor workContextAccessor, 
                    IEnumerable<IShapeDisplayEvents> shapeDisplayEvents, 
                    Lazy<IShapeTableLocator> shapeTableLocator, 
                    IPerformanceMonitor performanceMonitor, 
                    IOutputCachedPartsService outputCachedPartsService) 
            : base(workContextAccessor, shapeDisplayEvents, shapeTableLocator, performanceMonitor) 
        {
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