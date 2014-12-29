using Orchard;

namespace CJP.OutputCachedParts.Providers {
    public class CurrentThemeCacheKeyCompositeProvider : ICacheKeyCompositeProvider 
    {
        private readonly IOrchardServices _orchardServices;

        public CurrentThemeCacheKeyCompositeProvider(IOrchardServices orchardServices) 
        {
            _orchardServices = orchardServices;
        }

        public string GetCompositeValue() {
            return _orchardServices.WorkContext.CurrentTheme.Id;
        }
    }
}