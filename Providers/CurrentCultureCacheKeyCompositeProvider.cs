using Orchard;

namespace CJP.OutputCachedParts.Providers {
    public class CurrentCultureCacheKeyCompositeProvider : ICacheKeyCompositeProvider 
    {
        private readonly IOrchardServices _orchardServices;

        public CurrentCultureCacheKeyCompositeProvider(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        public string Description { get { return "Differentiates the cache key based on the Culture of the current request"; } }

        public string GetCompositeValue() 
        {
            return _orchardServices.WorkContext.CurrentCulture;
        }
    }
}