using System.Collections.Generic;
using CJP.OutputCachedParts.Models;
using Orchard;

namespace CJP.OutputCachedParts.Services
{
    public interface ILayerRetrievalService : IDependency
    {
        IEnumerable<CachedLayerModel> GetLayers();
    }
}