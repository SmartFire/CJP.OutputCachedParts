using System;
using System.Collections.Generic;
using System.Web;
using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services 
{
    public interface IOutputCachedPartsService : IDependency
    {
        void InvalidateCachedOutputKey(string cacheKey);
        void InvalidateCachedOutput(string contentType);
        void InvalidateCachedOutput(int contentId);
        void InvalidateCachedOutput(ContentPart part);
        void InvalidateCachedOutput(IEnumerable<string> contentTypes);
        void InvalidateCachedOutput(IEnumerable<int> contentIds);
        void InvalidateCachedOutput(IEnumerable<ContentPart> parts);
        IHtmlString BuildAndCacheOutput(Func<IHtmlString> htmlStringFactory, ContentPart part);
    }
}