using System;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace CJP.OutputCachedParts.OutputCachedParts.DriverResults
{
    public class OutputCachedContentShapeResult : DriverResult
    {
        private string _defaultLocation;
        private string _differentiator;
        private readonly string _shapeType;
        private string _groupId;
        private readonly Func<DriverResult> _driverResultFactory;
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;
        private readonly CachedPartMetadata _cachedPartMetadata;

        public OutputCachedContentShapeResult(string shapeType, Func<DriverResult> driverResultFactory, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext, ContentPart part, string cacheKey, TimeSpan cacheDuration)
        {
            _cachedPartMetadata = new CachedPartMetadata(part, cacheKey, cacheDuration);
            _shapeType = shapeType;
            _driverResultFactory = driverResultFactory;
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
        }

        public OutputCachedContentShapeResult(string shapeType, Func<DriverResult> driverResultFactory, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext, ContentPart part, string cacheKey)
        {
            _cachedPartMetadata = new CachedPartMetadata(part, cacheKey);
            _shapeType = shapeType;
            _driverResultFactory = driverResultFactory;
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
        }

        public OutputCachedContentShapeResult(string shapeType, Func<DriverResult> driverResultFactory, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext, ContentPart part)
            : this(shapeType, driverResultFactory, cacheService,outputCachedPartsContext, part, shapeType)
        {}

        public override void Apply(BuildDisplayContext context)
        {
            ApplyImplementation(context, context.DisplayType);
        }

        private void ApplyImplementation(BuildDisplayContext context, string displayType) 
        {
            var placement = context.FindPlacement(_shapeType, _differentiator, _defaultLocation);
            if (string.IsNullOrEmpty(placement.Location) || placement.Location == "-")
                return;

            // parse group placement
            var group = placement.GetGroup();
            if (!String.IsNullOrEmpty(group))
            {
                _groupId = group;
            }

            if (!string.Equals(context.GroupId ?? "", _groupId ?? "", StringComparison.OrdinalIgnoreCase))
                return;

            dynamic parentShape = context.Shape;
            // the zone name is in reference of Layout, e.g. /AsideSecond
            if (placement.IsLayoutZone())
            {
                parentShape = context.Layout;
            }

            var position = placement.GetPosition();
            var zone = placement.GetZone();

            var cachedModel = _cacheService.Get<OutputCachedPartsModel>(_cachedPartMetadata.CacheKey);

            if (cachedModel != null)
            {
                if (String.IsNullOrEmpty(position))
                {
                    parentShape.Zones[zone].Add(context.New.CachedHtml(cachedModel: cachedModel, cacheKey: _cachedPartMetadata.CacheKey));
                }
                else
                {
                    parentShape.Zones[zone].Add(context.New.CachedHtml(cachedModel: cachedModel, cacheKey: _cachedPartMetadata.CacheKey), position);
                }

                return;
            }

            _outputCachedPartsContext.PutCachedPartMetadata(ContentPart, _cachedPartMetadata);
            _driverResultFactory().Apply(context);
        }

        public OutputCachedContentShapeResult Location(string zone)
        {
            _defaultLocation = zone;
            return this;
        }

        public OutputCachedContentShapeResult Differentiator(string differentiator)
        {
            _differentiator = differentiator;
            return this;
        }

        public OutputCachedContentShapeResult OnGroup(string groupId)
        {
            _groupId = groupId;
            return this;
        }

        public string GetDifferentiator()
        {
            return _differentiator;
        }

        public string GetGroup()
        {
            return _groupId;
        }

        public string GetLocation()
        {
            return _defaultLocation;
        }

        public string GetShapeType()
        {
            return _shapeType;
        }
    }
}