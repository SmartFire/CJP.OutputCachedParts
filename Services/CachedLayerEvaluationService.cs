using System;
using System.Collections.Generic;
using System.Linq;
using CJP.OutputCachedParts.Models;
using Glimpse.Orchard.Models;
using Glimpse.Orchard.Models.Messages;
using Glimpse.Orchard.PerformanceMonitors;
using Orchard;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Widgets.Models;
using Orchard.Widgets.Services;

namespace CJP.OutputCachedParts.Services
{
    [OrchardFeature("CJP.OutputCachedParts.CachedLayers")]
    [OrchardSuppressDependency("CJP.OutputCachedParts.Services.DefaultLayerEvaluationService")]
    public class CachedLayerEvaluationService : ILayerEvaluationService
    {
        private readonly ICacheService _cacheService;
        private readonly IOrchardServices _orchardServices;
        private readonly IWidgetsService _widgetsService;
        private readonly IPerformanceMonitor _performanceMonitor;
        private readonly IRuleManager _ruleManager;

        public CachedLayerEvaluationService(ICacheService cacheService, IOrchardServices orchardServices, IWidgetsService widgetsService, IPerformanceMonitor performanceMonitor, IRuleManager ruleManager) 
        {
            _cacheService = cacheService;
            _orchardServices = orchardServices;
            _widgetsService = widgetsService;
            _performanceMonitor = performanceMonitor;
            _ruleManager = ruleManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public int[] GetActiveLayerIds()
        {
            var populatedLayers = _cacheService.Get(PopulatedLayersCacheKey, () =>
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

            var activeLayerIds = new List<int>();
            foreach (var layer in populatedLayers)
            {
                // ignore the rule if it fails to execute
                try
                {
                    var currentLayer = layer;
                    var layerRuleMatches = _performanceMonitor.PublishTimedAction(() => _ruleManager.Matches(currentLayer.Rule), (r, t) => new LayerMessage
                    {
                        Active = r,
                        Name = currentLayer.Name,
                        Rule = currentLayer.Rule,
                        Duration = t.Duration
                    }, TimelineCategories.Layers, "Layer Evaluation", currentLayer.Name).ActionResult;

                    if (layerRuleMatches)
                    {
                        activeLayerIds.Add(currentLayer.Id);
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e, T("An error occurred during layer evaluation on: {0}. The rule that was attempted was {1}.", layer.Name, layer.Rule).Text);
                }
            }

            return activeLayerIds.ToArray();
        }

        public static string AllLayersCacheKey { get { return "CJP.OutputCachedParts.AllLayers"; } }
        public static string WidgetContainersCacheKey { get { return "CJP.OutputCachedParts.WidgetContainers"; } }
        public static string PopulatedLayersCacheKey { get { return "CJP.OutputCachedParts.PopulatedLayers"; } }
    }
}