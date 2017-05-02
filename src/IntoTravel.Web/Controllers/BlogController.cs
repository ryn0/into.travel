using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class BlogController : Controller
    {
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

            return View(Helpers.ModelConverter.Convert(model));
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
