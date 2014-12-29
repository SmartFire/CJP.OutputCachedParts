using CJP.OutputCachedParts.OutputCachedParts.DriverResults;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Navigation.Drivers;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace CJP.OutputCachedParts.OutputCachedParts.AlternateImplementations.Drivers
{
    [OrchardSuppressDependency("Orchard.Core.Navigation.Drivers.MenuWidgetPartDriver")]
    public class OutputCachedMenuWidgetPartDriver : MenuWidgetPartDriver
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;

        public OutputCachedMenuWidgetPartDriver(
            IContentManager contentManager,
            INavigationManager navigationManager,
            IWorkContextAccessor workContextAccessor,
            IMenuService menuService,
            ICacheService cacheService,
            IOutputCachedPartsContext outputCachedPartsContext)
            : base(contentManager, navigationManager, workContextAccessor, menuService)
        {
            _workContextAccessor = workContextAccessor;
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(MenuWidgetPart part, string displayType, dynamic shapeHelper)
        {
            var currentCulture = _workContextAccessor.GetContext().CurrentCulture;
            var request = _workContextAccessor.GetContext().HttpContext.Request;
            var cacheKey = string.Format("MenuWidget.{0}", currentCulture);
            if (request != null)
            {
                cacheKey = string.Format("{0}.{1}.{2}", cacheKey, request.Path, request.ApplicationPath);
            }

            return new OutputCachedContentShapeResult("Parts_MenuWidget", () => base.Display(part, displayType, (object)shapeHelper), _cacheService, _outputCachedPartsContext, part, cacheKey);
        }
    }
}
