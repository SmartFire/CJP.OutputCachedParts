using System;
using System.Collections.Generic;
using System.Linq;
using CJP.OutputCachedParts.Extensions;
using CJP.OutputCachedParts.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Environment.Extensions;

namespace CJP.OutputCachedParts.Handlers
{
    [OrchardFeature("CJP.OutputCachedParts.MenuWidgetPart")]
    public class MenuPartInvalidationHandler : ContentHandler
    {
        private readonly IOutputCachedPartsService _outputCachedPartsService;
        private readonly IContentManager _contentManager;
        private readonly IMenuService _menuService;

        public MenuPartInvalidationHandler(IOutputCachedPartsService outputCachedPartsService, IContentManager contentManager, IMenuService menuService)
        {
            _outputCachedPartsService = outputCachedPartsService;
            _contentManager = contentManager;
            _menuService = menuService;
        }

        protected override void Created(CreateContentContext context)
        {
            base.Created(context);

            InvalidateCachesAfteMenuItemChanges(context);
        }

        protected override void Published(PublishContentContext context)
        {
            base.Published(context);

            InvalidateCachesAfteMenuItemChanges(context);
        }

        protected override void Updated(UpdateContentContext context)
        {
            base.Updated(context);

            InvalidateCachesAfteMenuItemChanges(context);
        }

        protected override void Removing(RemoveContentContext context)
        {
            base.Removing(context);

            InvalidateCachesAfteMenuItemChanges(context);
        }

        private void InvalidateCachesAfteMenuItemChanges(ContentContextBase context)
        {
            var stereotype = context.ContentItem.GetStereotype();

            if (string.Equals(stereotype, "MenuItem", StringComparison.InvariantCultureIgnoreCase))
            {
                var menuItemId = context.ContentItem.Id;

                //get all menu ids
                var allMenuIds = _contentManager.Query("Menu").List().Select(ci => ci.Id);
                var relevantMenuIds = new List<int>();

                //foreach menu id, check to see if the menu contains this menu part
                foreach (var menuId in allMenuIds)
                {
                    if (_menuService.GetMenuParts(menuId).Select(p => p.Id).Contains(menuItemId))
                    {
                        relevantMenuIds.Add(menuId);
                    }
                }

                //if so, get all menu widgets that use this menu and invalidate cache
                foreach (var menuId in relevantMenuIds)
                {
                    var scopedMenuId = menuId;
                    var menuWidgets = _contentManager.Query("MenuWidget").List().Select(w=> w.As<MenuWidgetPart>()).Where(p=>p!=null && p.MenuContentItemId == scopedMenuId);

                    _outputCachedPartsService.InvalidateCachedOutput(menuWidgets);
                }
            }
        }
    }
}