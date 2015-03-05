using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.Logging;
using Orchard.UI.Resources;

namespace CJP.OutputCachedParts.OutputCachedParts.Shapes 
{
    public class CachedShapes : IDependency 
    {
        private readonly Work<IResourceManager> _resourceManager;

        public ILogger Logger { get; set; }

        public CachedShapes(Work<IResourceManager> resourceManager) 
        {
            _resourceManager = resourceManager;

            Logger = NullLogger.Instance;
        }

        [Shape]
        public IHtmlString CachedHtml(OutputCachedPartsModel cachedModel, string cacheKey) 
        {
            if (cachedModel == null)
            {
                Logger.Error("Could not render Cached Html Shape for cache key {0} because the OutputCachedPartsModel was null.", cacheKey);

                return new HtmlString(string.Empty);
            }

            var resourceManager = _resourceManager.Value;

            if (resourceManager == null &&
                    (
                        (IsNotNullOrEmpty(cachedModel.IncludedResources)) ||
                        (IsNotNullOrEmpty(cachedModel.RequiredResources)) ||
                        (IsNotNullOrEmpty(cachedModel.HeadScripts)) ||
                        (IsNotNullOrEmpty(cachedModel.FootScripts))
                    )
               ) 
            {
                Logger.Error("Could not render Cached Html Shape for cache key {0} because the IResourceManager was required and it was null.", cacheKey);

                return new HtmlString(string.Empty);
            }


            if (IsNotNullOrEmpty(cachedModel.IncludedResources))
            {
                foreach (var resource in cachedModel.IncludedResources)
                {
                    resourceManager.Include(resource.ResourceType, resource.ResourcePath, resource.ResourceDebugPath, resource.RelativeFromPath);
                }
            }

            if (IsNotNullOrEmpty(cachedModel.RequiredResources))
            {
                foreach (var resource in cachedModel.RequiredResources)
                {
                    resourceManager.Require(resource.ResourceType, resource.ResourceName);
                }
            }

            if (IsNotNullOrEmpty(cachedModel.HeadScripts))
            {
                foreach (var script in cachedModel.HeadScripts)
                {
                    resourceManager.RegisterHeadScript(script);
                }
            }

            if (IsNotNullOrEmpty(cachedModel.FootScripts))
            {
                foreach (var script in cachedModel.FootScripts)
                {
                    resourceManager.RegisterFootScript(script);
                }
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

        private bool IsNotNullOrEmpty<T>(IEnumerable<T> target) 
        {
            return target != null && target.Any();
        }
    }
}