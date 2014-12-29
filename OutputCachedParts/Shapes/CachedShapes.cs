using System.Text;
using System.Web;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.UI.Resources;

namespace CJP.OutputCachedParts.OutputCachedParts.Shapes 
{
    public class CachedShapes : IDependency 
    {
        private readonly Work<IResourceManager> _resourceManager;

        public CachedShapes(Work<IResourceManager> resourceManager) 
        {
            _resourceManager = resourceManager;
        }

        [Shape]
        public IHtmlString CachedHtml(OutputCachedPartsModel cachedModel, string cacheKey)
        {
            foreach (var resource in cachedModel.IncludedResources)
            {
                _resourceManager.Value.Include(resource.ResourceType, resource.ResourcePath, resource.ResourceDebugPath, resource.RelativeFromPath);
            }

            foreach (var resource in cachedModel.RequiredResources)
            {
                _resourceManager.Value.Require(resource.ResourceType, resource.ResourceName);
            }

            foreach (var script in cachedModel.HeadScripts)
            {
                _resourceManager.Value.RegisterHeadScript(script);
            }

            foreach (var script in cachedModel.FootScripts)
            {
                _resourceManager.Value.RegisterFootScript(script);
            }

            var sb = new StringBuilder();
            sb.AppendLine(@"<!-- BEGIN OUTPUT CACHED PART:");
            sb.AppendLine(string.Format(@"      Cache Key: {0}", cacheKey));
            sb.AppendLine(string.Format(@"      Date/Time Generated (UTC): {0}", cachedModel.GenerationDateTime));
            sb.AppendLine(@"-->");
            sb.AppendLine(cachedModel.Html);
            sb.AppendLine(string.Format(@"<!--    END OUTPUT CACHED PART: {0} -->", cacheKey));

            return new HtmlString(sb.ToString());
        }
    }
}