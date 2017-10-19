using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Helpers;
using Microsoft.Extensions.Caching.Memory;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Controllers
{
    public class BlogController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryRepository _blogEntryRepository;
        private readonly IMemoryCache _memoryCache;

        public BlogController(
            IBlogEntryRepository blogEntryRepository,
            IMemoryCache memoryCache)
        {
            _blogEntryRepository = blogEntryRepository;
            _memoryCache = memoryCache;
        }

        [Route("blog/{year}/{month}/{day}/{key}")]
        [HttpGet]
        public IActionResult Display(string year, string month, string day, string key)
        {
            var cacheKey = $"{year}/{month}/{day}/{key}";
            BlogEntryDisplayModel model;
            var cachedPage = _memoryCache.Get(cacheKey);

            if (cachedPage == null)
            {
                var dbModel = _blogEntryRepository.Get(key);

                if (dbModel == null)
                {
                    Response.StatusCode = 404;

                    return View("Page404");
                }

                ValidateRequest(year, month, day, dbModel);

                var previous = _blogEntryRepository.GetPreviousEntry(dbModel.BlogPublishDateTimeUtc);
                var next = _blogEntryRepository.GetNextEntry(dbModel.BlogPublishDateTimeUtc);
                model = ModelConverter.ConvertToBlogDisplayModel(dbModel, previous, next);

                _memoryCache.Set(cacheKey, model, DateTime.UtcNow.AddMinutes(10));
            }
            else
            {
                model = (BlogEntryDisplayModel)cachedPage;
            }

            return View("DisplayBlog", model);
        }

        [Route("blog/page/{pageNumber}")]
        [HttpGet]
        public IActionResult Page(int pageNumber = 1)
        {
            int total;

            var model = ModelConverter.BlogPage(_blogEntryRepository.GetLivePage(pageNumber, AmountPerPage, out total), pageNumber, AmountPerPage, total);

            ViewData["Title"] = Data.Constants.StringConstants.DefaultPageTitle;
            ViewData["MetaDescription"] = Data.Constants.StringConstants.DefaultPageDescription;

            return View("BlogList", model);
        }

        private static void ValidateRequest(string year, string month, string day, Data.Models.BlogEntry model)
        {
            if (!model.IsLive ||
                 model.BlogPublishDateTimeUtc > DateTime.UtcNow ||
                 model.BlogPublishDateTimeUtc.Year.ToString("0000") != year ||
                 model.BlogPublishDateTimeUtc.Month.ToString("00") != month ||
                 model.BlogPublishDateTimeUtc.Day.ToString("00") != day)
                throw new Exception("not available");
        }
    }
}
