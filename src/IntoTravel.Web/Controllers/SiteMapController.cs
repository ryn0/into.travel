using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Helpers;

namespace IntoTravel.Web.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly IBlogEntryRepository _blogEntryRepository;

        public SiteMapController(IBlogEntryRepository blogEntryRepository)
        {
            _blogEntryRepository = blogEntryRepository;
        }

        [Route("/sitemap.xml")]
        public IActionResult Index()
        {
            int total;
            var allBlogs = _blogEntryRepository.GetLivePage(1, int.MaxValue, out total);

            var siteMapHelper = new SiteMapHelper();

            foreach (var blog in allBlogs)
            {
                if (!blog.IsLive)
                    continue;

                var url = new Uri(HttpContext.Request.Scheme +
                                 "://" +
                                 HttpContext.Request.Host.ToUriComponent() +
                                 UrlBuilder.BlogUrlPath(blog.Key, blog.BlogPublishDateTimeUtc));

                var lastUpdated = blog.UpdateDate == null ? blog.CreateDate : (DateTime)blog.UpdateDate;
                siteMapHelper.AddUrl(url.ToString(), lastUpdated, ChangeFrequency.weekly, .5);
            }

            var xml = siteMapHelper.GenerateXml();

            return Content(xml, "text/xml");
        }
 
    }
}
