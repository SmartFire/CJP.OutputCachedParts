using System;
using System.Collections.Generic;
using CJP.OutputCachedParts.OutputCachedParts.Events;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.OutputCachedParts.Services
{
    public class OutputCachedPartsContext : IOutputCachedPartsContext, IResourceManagerEvents
    {
        private bool WithinContext { get; set; }
        private IList<ResourceRequiredModel> RequiredResources { get; set; }
        private IList<ResourceIncludedModel> IncludedResources { get; set; }
        private IList<string> FootScripts { get; set; }
        private IList<string> HeadScripts { get; set; }
        private IDictionary<string, CachedPartMetadata> CachedPartMetadata { get; set; }

        public OutputCachedPartsContext() 
        {
            CachedPartMetadata = new Dictionary<string, CachedPartMetadata>();
        }

        private string BuildCachedPartMetadataKey(ContentPart part) 
        {
            return string.Format("{0}-{1}", part.TypePartDefinition.PartDefinition.Name, part.Id);
        }

        public void PutCachedPartMetadata(ContentPart part, CachedPartMetadata metadata) 
        {
            CachedPartMetadata.Add(new KeyValuePair<string, CachedPartMetadata>(BuildCachedPartMetadataKey(part), metadata));
        }

        public CachedPartMetadata GetCachedPartMetadata(ContentPart part) 
        {
            var dictionaryKey = BuildCachedPartMetadataKey(part);

            if (CachedPartMetadata.ContainsKey(dictionaryKey)) 
            {
                return CachedPartMetadata[dictionaryKey];
            }

            return null;
        }

        public OutputCachedPartsModel GetCacheModel(Func<string> htmlFactory)
        {
            WithinContext = true;
            RequiredResources = new List<ResourceRequiredModel>();
            IncludedResources = new List<ResourceIncludedModel>();
            FootScripts = new List<string>();
            HeadScripts = new List<string>();

            var model = new OutputCachedPartsModel {
                Html = htmlFactory()
            };

            model.IncludedResources = IncludedResources;
            model.RequiredResources = RequiredResources;
            model.FootScripts = FootScripts;
            model.HeadScripts = HeadScripts;

            WithinContext = false;

            return model;
        }

        //when these events are raised while creating html to be cached, then we need to add a record of the values passed into the cache as well so that we can re-instate the resources when we get the html back from the cache
        public void FootScriptRegistered(string script)
        {
            if (WithinContext)
            {
                FootScripts.Add(script);
            }
        }

        public void HeadScriptRegistered(string script)
        {
            if (WithinContext)
            {
                HeadScripts.Add(script);
            }
        }

        public void ResourceRequired(string resourceType, string resourceName)
        {
            if (WithinContext) {
                RequiredResources.Add(new ResourceRequiredModel
                {
                    ResourceType = resourceType,
                    ResourceName = resourceName,
                });
            }
        }

        public void ResourceIncluded(string resourceType, string resourcePath, string resourceDebugPath, string relativeFromPath)
        {
            if (WithinContext)
            {
                IncludedResources.Add(new ResourceIncludedModel
                {
                    ResourceType = resourceType,
                    ResourcePath = resourcePath,
                    ResourceDebugPath = resourceDebugPath,
                    RelativeFromPath = relativeFromPath
                });
            }
        }
    }
}