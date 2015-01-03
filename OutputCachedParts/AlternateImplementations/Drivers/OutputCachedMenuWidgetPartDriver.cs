using CJP.OutputCachedParts.Services;
using Orchard;
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
    [OrchardFeature("CJP.OutputCachedParts.MenuWidgetPart")]
    public class OutputCachedMenuWidgetPartDriver : MenuWidgetPartDriver
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IOutputCachedDriverResultFactory _driverResultFactory;

        public OutputCachedMenuWidgetPartDriver(
            IContentManager contentManager,
            INavigationManager navigationManager,
            IWorkContextAccessor workContextAccessor,
            IMenuService menuService,
            IOutputCachedDriverResultFactory driverResultFactory)
            : base(contentManager, navigationManager, workContextAccessor, menuService)
        {
            _workContextAccessor = workContextAccessor;
            _driverResultFactory = driverResultFactory;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(MenuWidgetPart part, string displayType, dynamic shapeHelper)
        {
            var request = _workContextAccessor.GetContext().HttpContext.Request;
            var cacheKey = "MenuWidget";
            if (request != null)
            {
                cacheKey = string.Format("{0}.{1}.{2}", cacheKey, request.Path, request.ApplicationPath);
            }

            return _driverResultFactory.BuildResult(part, "Parts_MenuWidget", () => base.Display(part, displayType, (object)shapeHelper), cacheKey);
        }
    }
}
