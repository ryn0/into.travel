using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Helpers;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Controllers
{
    public class TagController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryRepository _blogEntryRepository;

        public TagController(IBlogEntryRepository blogEntryRepository)
        {
            _blogEntryRepository = blogEntryRepository;
        }

        [Route("tag/{keyword}")]
        [HttpGet]
        public IActionResult Index(string keyword, int pageNumber = 1)
        {
            var model = SetModel(keyword, pageNumber);

            return View("BlogList", model);
        }

        [Route("tag/{keyword}/page/{pageNumber}")]
        [HttpGet]
        public IActionResult Page(string keyword, int pageNumber = 1)
        {
            var model = SetModel(keyword, pageNumber);

            return View("BlogList", model);
        }

        private BlogEntryDisplayListModel SetModel(string keyword, int pageNumber)
        {
            keyword = keyword.Replace("-", " ");

            int total;

            var model = ModelConverter.BlogPage(_blogEntryRepository.GetLivePageByTag(keyword, pageNumber, AmountPerPage, out total), pageNumber, AmountPerPage, total);

            ViewBag.TagKeyword = keyword;
            return model;
        }
    }
}
