using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJP.OutputCachedParts.Models;
using Glimpse.Orchard.Models;
using Glimpse.Orchard.PerfMon.Services;
using Glimpse.Orchard.Tabs.Layers;
using Glimpse.Orchard.Tabs.Widgets;
using Orchard;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Settings.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Filters;
using Orchard.Themes;
using Orchard.UI.Admin;
using Orchard.Widgets.Models;
using Orchard.Widgets.Services;

namespace CJP.OutputCachedParts.Filters
{
    [OrchardSuppressDependency("Orchard.Widgets.Filters.WidgetFilter")]
    [OrchardSuppressDependency("Glimpse.Orchard.AlternateImplementations.GlimpseWidgetFilter")]
    [OrchardFeature("CJP.OutputCachedParts.CachedWidgetFilter")]
    public class CachedWidgetFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRuleManager _ruleManager;
        private readonly IWidgetsService _widgetsService;
        private readonly IOrchardServices _orchardServices;
        private readonly ICacheService _cacheService;
        private readonly IPerformanceMonitor _performanceMonitor;

        public CachedWidgetFilter(IWorkContextAccessor workContextAccessor, 
            IRuleManager ruleManager, 
            IWidgetsService widgetsService, 
            IOrchardServices orchardServices,
            ICacheService cacheService,
            IPerformanceMonitor performanceMonitor) 
        {
            _workContextAccessor = workContextAccessor;
            _ruleManager = ruleManager;
            _widgetsService = widgetsService;
            _orchardServices = orchardServices;
            _cacheService = cacheService;
            _performanceMonitor = performanceMonitor;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; private set; }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // layers and widgets should only run on a full view rendering result
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;

            var workContext = _workContextAccessor.GetContext(filterContext);

            if (workContext == null ||
                workContext.Layout == null ||
                workContext.CurrentSite == null ||
                AdminFilter.IsApplied(filterContext.RequestContext) ||
                !ThemeFilter.IsApplied(filterContext.RequestContext))
            {
                return;
            }

            var layers = _cacheService.Get(AllLayersCacheKey, () => 
                _orchardServices.ContentManager.Query<LayerPart, LayerPartRecord>().List().Select(p => new CachedLayerModel {
                    Id = p.Id,
                    Name = p.Name,
                    Rule = p.LayerRule
                })
            );

            var activeLayerIds = new List<int>();
            foreach (var layer in layers)
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

            IEnumerable<WidgetPart> widgetParts = _widgetsService.GetWidgets(layerIds: activeLayerIds.ToArray());

            // Build and add shape to zone.
            var zones = workContext.Layout.Zones;
            var defaultCulture = workContext.CurrentSite.As<SiteSettingsPart>().SiteCulture;
            var currentCulture = workContext.CurrentCulture;

            foreach (var widgetPart in widgetParts)
            {
                var commonPart = widgetPart.As<ICommonPart>();
                if (commonPart == null || commonPart.Container == null)
                {
                    Logger.Warning("The widget '{0}' is has no assigned layer or the layer does not exist.", widgetPart.Title);
                    continue;
                }

                // ignore widget for different cultures
                var localizablePart = widgetPart.As<ILocalizableAspect>();
                if (localizablePart != null)
                {
                    // if localized culture is null then show if current culture is the default
                    // this allows a user to show a content item for the default culture only
                    if (localizablePart.Culture == null && defaultCulture != currentCulture)
                    {
                        continue;
                    }

                    // if culture is set, show only if current culture is the same
                    if (localizablePart.Culture != null && localizablePart.Culture != currentCulture)
                    {
                        continue;
                    }
                }

                // check permissions
                if (!_orchardServices.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, widgetPart))
                {
                    continue;
                }

                var scopedWidgetPart = widgetPart;
                var widgetBuildDisplayTime = _performanceMonitor.Time(() => _orchardServices.ContentManager.BuildDisplay(scopedWidgetPart));
                var widgetShape = widgetBuildDisplayTime.ActionResult;

                _performanceMonitor.PublishMessage(new WidgetMessage
                {
                    Title = widgetPart.Title,
                    Type = widgetPart.ContentItem.ContentType,
                    Zone = widgetPart.Zone,
                    Layer = widgetPart.LayerPart,
                    Position = widgetPart.Position,
                    TechnicalName = widgetPart.Name,
                    Duration = widgetBuildDisplayTime.TimerResult.Duration
                });

                zones[widgetPart.Record.Zone].Add(widgetShape, widgetPart.Record.Position);
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {}

        public static string AllLayersCacheKey { get { return "CJP.OutputCachedParts.AllLayers"; } }
        public static string PopulatedLayersCacheKey { get { return "CJP.OutputCachedParts.PopulatedLayers"; } }
    }
}