using System.Collections.Generic;
using Autofac.Features.Metadata;
using CJP.OutputCachedParts.OutputCachedParts.Events;
using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace CJP.OutputCachedParts.OutputCachedParts.AlternateImplementations
{
    [OrchardSuppressDependency("Orchard.UI.Resources.ResourceManager")]
    public class EventRaisingResourceManager : ResourceManager
    {
        private readonly IResourceManagerEvents _resourceManagerEvents;

        private bool SuppressRequireEvents { get; set; }

        public EventRaisingResourceManager(IEnumerable<Meta<IResourceManifestProvider>> resourceProviders, IResourceManagerEvents resourceManagerEvents) 
            : base(resourceProviders) {
            _resourceManagerEvents = resourceManagerEvents;
        }

        public override void RegisterFootScript(string script)
        {
            _resourceManagerEvents.FootScriptRegistered(script);
            base.RegisterFootScript(script);
        }

        public override void RegisterHeadScript(string script)
        {
            _resourceManagerEvents.HeadScriptRegistered(script);
            base.RegisterHeadScript(script);
        }

        public override RequireSettings Require(string resourceType, string resourceName) {
            //include calls require under the hood, so we will end up with dupicate events in that case
            if (!SuppressRequireEvents) {
                _resourceManagerEvents.ResourceRequired(resourceType, resourceName);
            }

            return base.Require(resourceType, resourceName);
        }

        public override RequireSettings Include(string resourceType, string resourcePath, string resourceDebugPath, string relativeFromPath)
        {
            _resourceManagerEvents.ResourceIncluded(resourceType, resourcePath, resourceDebugPath, relativeFromPath);

            SuppressRequireEvents = true;
            var result = base.Include(resourceType, resourcePath, resourceDebugPath, relativeFromPath);
            SuppressRequireEvents = false;

            return result;
        }
    }
}