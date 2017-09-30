using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Helpers;

namespace IntoTravel.Web.Controllers
{
    public class BlogController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryRepository _blogEntryRepository;

        public BlogController(IBlogEntryRepository blogEntryRepository)
        {
            _blogEntryRepository = blogEntryRepository;
        }

        [Route("blog/{year}/{month}/{day}/{key}")]
        [HttpGet]
        public IActionResult Display(string year, string month, string day, string key)
        {
            var model = _blogEntryRepository.Get(key);

            ValidateRequest(year, month, day, model);

            var previous = _blogEntryRepository.GetPreviousEntry(model.BlogPublishDateTimeUtc);
            var next = _blogEntryRepository.GetNextEntry(model.BlogPublishDateTimeUtc);

            return View("DisplayBlog", ModelConverter.ConvertToBlogDisplayModel(model, previous, next));
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
