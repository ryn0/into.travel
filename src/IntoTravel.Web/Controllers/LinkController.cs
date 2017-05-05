using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IntoTravel.Web.Controllers
{
    public class LinkController : Controller
    {
        public const string CachePrefix = "link-";

        IMemoryCache _memoryCache;

        public LinkController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
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
            throw new Exception();

            //var cacheKey = CachePrefix + key;


            //if (_memoryCache.TryGetValue(cacheKey, out existingBadUsers))
            //{
            //    var cachedUserIds = existingBadUsers;
            //}
            //else
            //{
            //    _memoryCache.Set(cacheKey, badUserIds);
            //}
           
        }
    }
}
