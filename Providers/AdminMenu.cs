using Orchard.Localization;
using Orchard.UI.Navigation;

namespace CJP.OutputCachedParts.Providers
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Performance"), "4", item => item.Action("Index", "Admin", new { area = "CJP.OutputCachedParts" }).Permission(Permissions.ManageCachedKeys));
        }
    }
}