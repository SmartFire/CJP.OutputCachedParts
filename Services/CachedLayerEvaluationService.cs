using System;
using System.Collections.Generic;
using Glimpse.Orchard.Models;
using Glimpse.Orchard.Models.Messages;
using Glimpse.Orchard.PerformanceMonitors;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Widgets.Services;

namespace CJP.OutputCachedParts.Services
{
    [OrchardFeature("CJP.OutputCachedParts.CachedLayers")]
    [OrchardSuppressDependency("Orchard.Widgets.Services.DefaultLayerEvaluationService")]
    [OrchardSuppressDependency("Glimpse.Orchard.AlternateImplementations.GlimpseLayerEvaluationService")]
    public class CachedLayerEvaluationService : ILayerEvaluationService
    {
        private readonly IPerformanceMonitor _performanceMonitor;
        private readonly IRuleManager _ruleManager;
        private readonly ILayerRetrievalService _layerRetrievalService;

        private readonly LazyField<int[]> _activeLayerIds; 

        public CachedLayerEvaluationService(IPerformanceMonitor performanceMonitor, IRuleManager ruleManager, ILayerRetrievalService layerRetrievalService) 
        {
            _performanceMonitor = performanceMonitor;
            _ruleManager = ruleManager;
            _layerRetrievalService = layerRetrievalService;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;

            _activeLayerIds = new LazyField<int[]>();
            _activeLayerIds.Loader(PopulateActiveLayers);
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public int[] GetActiveLayerIds()
        {
            return _activeLayerIds.Value;
        }

        private int[] PopulateActiveLayers()
        {
            var populatedLayers = _layerRetrievalService.GetLayers();

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
    }
}