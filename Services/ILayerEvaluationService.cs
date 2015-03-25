using Orchard;

namespace CJP.OutputCachedParts.Services 
{
    public interface ILayerEvaluationService : IDependency 
    {
        int[] GetActiveLayerIds();
    }
}