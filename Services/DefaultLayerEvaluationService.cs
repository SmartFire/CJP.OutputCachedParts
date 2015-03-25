using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Widgets.Models;
using Orchard.Widgets.Services;

namespace CJP.OutputCachedParts.Services 
{
    [OrchardFeature("CJP.OutputCachedParts.CachedWidgetFilter")]
    public class DefaultLayerEvaluationService : ILayerEvaluationService 
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRuleManager _ruleManager;

        public DefaultLayerEvaluationService(IOrchardServices orchardServices, IRuleManager ruleManager) 
        {
            _orchardServices = orchardServices;
            _ruleManager = ruleManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public int[] GetActiveLayerIds()
        {
            // Once the Rule Engine is done:
            // Get Layers and filter by zone and rule
            IEnumerable<LayerPart> activeLayers = _orchardServices.ContentManager.Query<LayerPart, LayerPartRecord>().List();

            var activeLayerIds = new List<int>();
            foreach (var activeLayer in activeLayers)
            {
                // ignore the rule if it fails to execute
                try
                {
                    if (_ruleManager.Matches(activeLayer.Record.LayerRule))
                    {
                        activeLayerIds.Add(activeLayer.ContentItem.Id);
                    }
                }
                catch (Exception e)
                {
                    Logger.Warning(e, T("An error occured during layer evaluation on: {0}", activeLayer.Name).Text);
                }
            }

            return activeLayerIds.ToArray();
        }
    }
}