using System;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.OutputCachedParts.Services 
{
    public interface IOutputCachedPartsContext : IDependency
    {
        void PutCachedPartMetadata(ContentPart part, CachedPartMetadata metadata);
        CachedPartMetadata GetCachedPartMetadata(ContentPart part);
        OutputCachedPartsModel GetCacheModel(Func<string> htmlFactory);
    }
}