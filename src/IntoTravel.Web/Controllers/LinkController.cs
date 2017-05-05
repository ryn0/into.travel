using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IntoTravel.Data.Repositories.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class LinkController : Controller
    {
        public const string CachePrefix = "link-";

        IMemoryCache _memoryCache;
        private readonly ILinkRedirectionRepository _linkRedirectionRepository;

        public LinkController(
            IMemoryCache memoryCache, 
            ILinkRedirectionRepository linkRedirectionRepository)
        {
            _memoryCache = memoryCache;
            _linkRedirectionRepository = linkRedirectionRepository;
        }

        [Route("/go/{key}")]
        public ActionResult Go(string key)
        {
            var url = GetLinkForKey(key);

            if (url == null)
                Redirect("~/");

            return Redirect(url);
        }

        private string GetLinkForKey(string key)
        {
            string destination;
            var cacheKey = CachePrefix + key;

            if (_memoryCache.TryGetValue(cacheKey, out destination))
            {
                return destination;
            }
            else
            {
                var link = _linkRedirectionRepository.Get(key);

                _memoryCache.Set(cacheKey, link.UrlDestination);

                return link.UrlDestination;
            }

        }
    }
}
