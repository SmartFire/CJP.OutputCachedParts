using System.Collections.Generic;

namespace CJP.OutputCachedParts.Models {
    public class AdminIndexVM
    {
        public IList<OutputCachedPartSummary> OutputCachedPartSummaries { get; set; }
        public IList<CacheKeyCompositeProviderSummary> CompositeProviders { get; set; }
    }
}