using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJP.OutputCachedParts.Models;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.Data;

namespace CJP.OutputCachedParts.Services
{
    public class DefaultOutputCachedPartsService : IOutputCachedPartsService
    {
        private readonly IRepository<CacheKeyRecord> _cacheKeyRepository;
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;
        private readonly IContentManager _contentManager;

        public DefaultOutputCachedPartsService(IRepository<CacheKeyRecord> cacheKeyRepository, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext, IContentManager contentManager) 
        {
            _cacheKeyRepository = cacheKeyRepository;
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
            _contentManager = contentManager;
        }

        public void InvalidateCachedOutputKey(string cacheKey)
        {
            _cacheService.Remove(cacheKey);
        }

        public void InvalidateCachedOutput(string contentType)
        {
            InvalidateCachedOutput(new[] { contentType });
        }

        public void InvalidateCachedOutput(int contentId)
        {
            InvalidateCachedOutput(new[] { contentId });
        }

        public void InvalidateCachedOutput(ContentPart part)
        {
            InvalidateCachedOutput(new[] { part });
        }

        public void InvalidateCachedOutput(IEnumerable<string> contentTypes) 
        {
            var contentIds = _contentManager.Query(contentTypes.ToArray()).List().Select(c => c.Id);

            InvalidateCachedOutput(contentIds);
        }

        public void InvalidateCachedOutput(IEnumerable<int> contentIds)
        {
            var cackeKeys = _cacheKeyRepository.Fetch(r => contentIds.ToList().Contains(r.ContentId)).Select(r => r.CacheKey);

            foreach (var cackeKey in cackeKeys)
            {
                InvalidateCachedOutputKey(cackeKey);
            }
        }

        public void InvalidateCachedOutput(IEnumerable<ContentPart> parts)
        {
            InvalidateCachedOutput(parts.Select(p=>p.ContentItem.Id));
        }

        public IHtmlString BuildAndCacheOutput(Func<IHtmlString> htmlStringFactory, ContentPart part)
        {
            var cachedPartMetadata = _outputCachedPartsContext.GetCachedPartMetadata(part);

            if (cachedPartMetadata == null) 
            {
                return htmlStringFactory();
            }

            var cachedModel = _outputCachedPartsContext.GetCacheModel(() => htmlStringFactory().ToHtmlString());

            if (cachedPartMetadata.Timespan.HasValue)
            {
                _cacheService.Put(cachedPartMetadata.CacheKey, cachedModel, cachedPartMetadata.Timespan.Value);
            }
            else
            {
                _cacheService.Put(cachedPartMetadata.CacheKey, cachedModel);
            }

            var currentRecords = _cacheKeyRepository.Fetch(r => r.CacheKey == cachedPartMetadata.CacheKey);

            foreach (var currentRecord in currentRecords)
            {
                _cacheKeyRepository.Delete(currentRecord);
            }

            _cacheKeyRepository.Create(new CacheKeyRecord {
                CacheKey = cachedPartMetadata.CacheKey,
                ContentId = part.Id,
                PartName = part.PartDefinition.Name
            });

            return new HtmlString(cachedModel.Html);
        }
    }
}