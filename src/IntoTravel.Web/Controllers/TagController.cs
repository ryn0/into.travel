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
        private readonly ITagRepository _tagRepository;


        public TagController(IBlogEntryRepository blogEntryRepository, ITagRepository tagRepository)
        {
            _blogEntryRepository = blogEntryRepository;
            _tagRepository = tagRepository;
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
            int total;

            var model = ModelConverter.BlogPage(_blogEntryRepository.GetLivePageByTag(keyword, pageNumber, AmountPerPage, out total), 
                                                pageNumber, 
                                                AmountPerPage, 
                                                total);

            if (model.Items != null && model.Items.Count > 0)
            {
                ViewBag.TagKeyword = _tagRepository.Get(keyword).Name;
            }

            return model;
        }
    }
}
