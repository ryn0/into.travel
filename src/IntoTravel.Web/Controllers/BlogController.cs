using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult Display(string key)
        {
            var model = _blogEntryRepository.Get(key);

            if (!model.IsLive ||
                 model.BlogPublishDateTimeUtc > DateTime.UtcNow)
                throw new Exception("not available");

            return View(IntoTravel.Web.Helpers.ModelConverter.Convert(model));
        }
        
    }
}
