using System.Collections.Generic;
using System.Linq;
using Orchard;
using CJP.OutputCachedParts.Models;
using Orchard.Caching.Services;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment.Extensions;
using Orchard.Widgets.Models;
using Orchard.Widgets.Services;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services
{
    [OrchardFeature("CJP.OutputCachedParts.CachedLayers")]
    public class CachedLayerRetrievalService : ILayerRetrievalService
    {
        private readonly ICacheService _cacheService;
        private readonly IOrchardServices _orchardServices;
        private readonly IWidgetsService _widgetsService;

        public CachedLayerRetrievalService(ICacheService cacheService, IOrchardServices orchardServices, IWidgetsService widgetsService)
        {
            _cacheService = cacheService;
            _orchardServices = orchardServices;
            _widgetsService = widgetsService;
        }

        public IEnumerable<CachedLayerModel> GetLayers()
        {
            return _cacheService.Get(PopulatedLayersCacheKey, () =>
            {
                //get all the layers (all layers and widget containers are individually cached because they can be invalidated independently of each other)
                var layers = _cacheService.Get(AllLayersCacheKey, () =>
                        _orchardServices.ContentManager.Query<LayerPart, LayerPartRecord>().List().Select(p => new CachedLayerModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Rule = p.LayerRule
                        })
                    );


                //get a collection of widget containers
                var widgetContainers = _cacheService.Get(WidgetContainersCacheKey, () =>
                {
                    var allWidgets = _widgetsService.GetWidgets(layers.Select(l => l.Id).ToArray());
                    var containerIds = new List<int>();

                    foreach (var widgetPart in allWidgets)
                    {
                        var commonPart = widgetPart.As<ICommonPart>();
                        if (commonPart != null && commonPart.Container != null)
                        {
                            containerIds.Add(commonPart.Container.Id);
                        }
                    }

                    return containerIds;
                });

                //return the layers that are also in the collection of widget containers
                return layers.Where(l => widgetContainers.Contains(l.Id));
            });
        }

        public static string AllLayersCacheKey { get { return "CJP.OutputCachedParts.AllLayers"; } }
        public static string WidgetContainersCacheKey { get { return "CJP.OutputCachedParts.WidgetContainers"; } }
        public static string PopulatedLayersCacheKey { get { return "CJP.OutputCachedParts.PopulatedLayers"; } }
    }
}