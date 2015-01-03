using System;
using System.Web;
using CJP.OutputCachedParts.Models;
using CJP.OutputCachedParts.OutputCachedParts.Services;
using Orchard.Caching.Services;
using Orchard.ContentManagement;
using Orchard.Data;

namespace CJP.OutputCachedParts.Services
{
    public class DefaultOutputCachedPartsService : IDefaultOutputCachedPartsService
    {
        private readonly IRepository<CacheKeyRecord> _cacheKeyRepository;
        private readonly ICacheService _cacheService;
        private readonly IOutputCachedPartsContext _outputCachedPartsContext;

        public DefaultOutputCachedPartsService(IRepository<CacheKeyRecord> cacheKeyRepository, ICacheService cacheService, IOutputCachedPartsContext outputCachedPartsContext) {
            _cacheKeyRepository = cacheKeyRepository;
            _cacheService = cacheService;
            _outputCachedPartsContext = outputCachedPartsContext;
        }

        public void InvalidateCachedOutput(string cacheKey) {
            throw new NotImplementedException();
        }

        public void InvalidateCachedOutput(params string[] contentTypes) {
            throw new NotImplementedException();
        }

        public void InvalidateCachedOutput(params int[] contentIds) {
            throw new NotImplementedException();
        }

        public void InvalidateCachedOutput(ContentPart contentPart) {
            throw new NotImplementedException();
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
                ContentId = part.Id
            });

            return new HtmlString(cachedModel.Html);
        }
    }
}