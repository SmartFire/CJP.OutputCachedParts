using System;
using System.Web;
using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Services 
{
    public interface IOutputCachedPartsService : IDependency
    {
        void InvalidateCachedOutput(string cacheKey);
        void InvalidateCachedOutput(params string[] contentTypes);
        void InvalidateCachedOutput(params int[] contentIds);
        void InvalidateCachedOutput(ContentPart contentPart);
        IHtmlString BuildAndCacheOutput(Func<IHtmlString> htmlStringFactory, ContentPart part);
    }
}