using CJP.OutputCachedParts.Services;
using Orchard.ContentManagement.Handlers;

namespace CJP.OutputCachedParts.Handlers
{
    public class OutputCachedPartInvalidationHandler : ContentHandler
    {
        private readonly IOutputCachedPartsService _outputCachedPartsService;

        public OutputCachedPartInvalidationHandler(IOutputCachedPartsService outputCachedPartsService)
        {
            _outputCachedPartsService = outputCachedPartsService;
        }

        protected override void Updated(UpdateContentContext context) {
            base.Updated(context);

            _outputCachedPartsService.InvalidateCachedOutput(context.ContentItem.Id);
        }
    }
}