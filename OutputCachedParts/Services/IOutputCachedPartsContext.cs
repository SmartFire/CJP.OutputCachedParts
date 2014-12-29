using System;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.OutputCachedParts.Services 
{
    public interface IOutputCachedPartsContext : IDependency
    {
        void PutCachedPartMetadata(ContentPart part, string cacheKey);
        void PutCachedPartMetadata(ContentPart part, string cacheKey, TimeSpan cacheDuration);
        CachedPartMetadata GetCachedPartMetadata(ContentPart part);
        OutputCachedPartsModel GetCacheModel(Func<string> htmlFactory);
    }
}