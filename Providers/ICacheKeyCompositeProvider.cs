using Orchard;

namespace CJP.OutputCachedParts.Providers
{
    public interface ICacheKeyCompositeProvider : IDependency 
    {
        string GetCompositeValue();
    }
}