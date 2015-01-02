using Orchard;

namespace CJP.OutputCachedParts.Providers {
    public class CurrentThemeCacheKeyCompositeProvider : ICacheKeyCompositeProvider 
    {
        private readonly IOrchardServices _orchardServices;

        public CurrentThemeCacheKeyCompositeProvider(IOrchardServices orchardServices) 
        {
            _orchardServices = orchardServices;
        }
        public string Description { get { return "Differentiates the cache key based on the currently active theme"; } }

        public string GetCompositeValue() {
            return _orchardServices.WorkContext.CurrentTheme.Id;
        }
    }
}