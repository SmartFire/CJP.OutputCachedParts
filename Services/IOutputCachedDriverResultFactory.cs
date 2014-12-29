using System;
using CJP.OutputCachedParts.OutputCachedParts.DriverResults;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace CJP.OutputCachedParts.Services 
{
    public interface IOutputCachedDriverResultFactory : IDependency
    {
        OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory);
        OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory, string cacheKey);
        OutputCachedContentShapeResult BuildResult(ContentPart part, string shapeType, Func<DriverResult> driverResultFactory, string cacheKey, TimeSpan cacheDuration);
    }
}