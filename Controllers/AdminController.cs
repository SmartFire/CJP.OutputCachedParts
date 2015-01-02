using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJP.OutputCachedParts.Models;
using CJP.OutputCachedParts.OutputCachedParts.Models;
using CJP.OutputCachedParts.Providers;
using Orchard;
using Orchard.Caching.Services;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Navigation;

namespace CJP.OutputCachedParts.Controllers 
{
    public class AdminController : Controller 
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IEnumerable<ICacheKeyCompositeProvider> _cacheKeyCompositeProviders;
        private readonly ICacheService _cacheService;
        private readonly IRepository<CacheKeyRecord> _cacheKeyRecordRepository;

        public AdminController(IOrchardServices orchardServices, IEnumerable<ICacheKeyCompositeProvider> cacheKeyCompositeProviders, ICacheService cacheService, IRepository<CacheKeyRecord> cacheKeyRecordRepository)
        {
            _orchardServices = orchardServices;
            _cacheKeyCompositeProviders = cacheKeyCompositeProviders;
            _cacheService = cacheService;
            _cacheKeyRecordRepository = cacheKeyRecordRepository;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageCachedKeys, T("You are not authorized to manage cached content part keys")))
                return new HttpUnauthorizedResult();

            var model = new AdminIndexVM {
                CompositeProviders = _cacheKeyCompositeProviders.Select(p => new CacheKeyCompositeProviderSummary {CurrentValue = p.GetCompositeValue(), Description = p.Description, Name = p.GetType().Name}),
                OutputCachedPartSummaries = _cacheKeyRecordRepository.Table.Select(r=> new OutputCachedPartSummary{CacheKey = r.CacheKey, ContentId = r.ContentId, CachedValue = _cacheService.Get<OutputCachedPartsModel>(r.CacheKey)})
            };

            return View(model);
        }
    }

    public class AdminIndexVM
    {
        public IEnumerable<OutputCachedPartSummary> OutputCachedPartSummaries { get; set; }
        public IEnumerable<CacheKeyCompositeProviderSummary> CompositeProviders { get; set; }
    }

    public class OutputCachedPartSummary
    {
        public string CacheKey { get; set; }
        public int ContentId { get; set; }
        public OutputCachedPartsModel CachedValue { get; set; }
    }

    public class CacheKeyCompositeProviderSummary
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CurrentValue { get; set; }
    }
}