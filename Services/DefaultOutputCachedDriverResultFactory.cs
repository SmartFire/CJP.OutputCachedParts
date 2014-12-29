using System;
using CJP.OutputCachedParts.OutputCachedParts.DriverResults;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace CJP.OutputCachedParts.Services
{
    public class DefaultOutputCachedDriverResultFactory : IOutputCachedDriverResultFactory
    {
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;
        public DefaultOutputCachedDriverResultFactory(IOutputCachedPartsContext outputCachedPartsContext, ICacheService cacheService) 
        {
            _outputCachedPartsContext = outputCachedPartsContext;
            _cacheService = cacheService;
        }

        public OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory)
        {
            return new OutputCachedContentShapeResult(shapeType, driverResultFactory, _cacheService, _outputCachedPartsContext, part);
        }

        public OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory, string cacheKey)
        {
            return new OutputCachedContentShapeResult(shapeType, driverResultFactory, _cacheService, _outputCachedPartsContext, part, cacheKey);
        }

        public OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory, string cacheKey, TimeSpan cacheDuration)
        {
            return new OutputCachedContentShapeResult(shapeType, driverResultFactory, _cacheService, _outputCachedPartsContext, part, cacheKey, cacheDuration);
        }
    }
}