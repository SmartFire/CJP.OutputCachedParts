using System;
using System.Web;
using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services 
{
    public interface IDefaultOutputCachedPartsService : IDependency
    {
        void InvalidateCachedOutput(string cacheKey);
        void InvalidateCachedOutput(params string[] contentTypes);
        void InvalidateCachedOutput(int contentId);
        void InvalidateCachedOutput(ContentPart contentPart);
        IHtmlString BuildAndCacheOutput(Func<IHtmlString> htmlStringFactory, ContentPart part);
    }
}