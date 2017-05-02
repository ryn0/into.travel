using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;
using IntoTravel.Core.Utilities;

namespace IntoTravel.Web.Controllers
{
    [Authorize]
    public class BlogManagementController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryRepository _blogEntryRepository;
 
        public BlogManagementController(IBlogEntryRepository blogEntryRepository)
        {
            _blogEntryRepository = blogEntryRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int total;

            var blogEntries = _blogEntryRepository.GetPage(pageNumber, AmountPerPage, out total);

            var model = ConvertToListModel(blogEntries);

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new BlogManagementEntryModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BlogManagementEntryModel model)
        {
            _blogEntryRepository.Create(new BlogEntry()
            {
                Content = model.Content,
                Title = model.Title,
                BlogPublishDateTimeUtc = model.BlogPublishDateTimeUtc,
                Key = model.Title.UrlKey()
            });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int blogEntryId)
        {
            var dbModel = _blogEntryRepository.Get(blogEntryId);

            var model = ToUiEditModel(dbModel);

            return View(model);
        }

       

        [HttpPost]
        public IActionResult Delete(int blogEntryId)
        {
            _blogEntryRepository.Delete(blogEntryId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(BlogManagementEntryModel model)
        {
            var dbModel = ConvertToDbModel(model);

            if (_blogEntryRepository.Update(dbModel))
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Route("blogmanagement/preview/{key}")]
        [HttpGet]
        public IActionResult Preview(string key)
        {
            var model = _blogEntryRepository.Get(key);

            return View(IntoTravel.Web.Helpers.ModelConverter.Convert(model));
        }

 
        private BlogManagementListModel ConvertToListModel(List<BlogEntry> blogEntries)
        {
            var model = new BlogManagementListModel();

            foreach(var entry in blogEntries)
            {
                model.Items.Add(new BlogManagementEntryItemModel()
                {
                    BlogEntryId = entry.BlogEntryId,
                    CreateDate = entry.CreateDate,
                    Title = entry.Title,
                    IsLive = entry.IsLive,
                    Key = entry.Key
                });
            }

            return model;
        }


        private BlogEntry ConvertToDbModel(BlogManagementEntryModel model)
        {
            var dbModel = _blogEntryRepository.Get(model.BlogEntryId);
            
            dbModel.BlogPublishDateTimeUtc = model.BlogPublishDateTimeUtc;
            dbModel.Content = model.Content;
            dbModel.Title = model.Title;
            dbModel.IsLive = model.IsLive;
            dbModel.Key = model.Title.UrlKey();

            return dbModel;
        }

        private static BlogManagementEntryModel ToUiEditModel(BlogEntry dbModel)
        {
            return new BlogManagementEntryModel()
            {
                Content = dbModel.Content,
                Title = dbModel.Title,
                BlogEntryId = dbModel.BlogEntryId,
                BlogPublishDateTimeUtc = dbModel.BlogPublishDateTimeUtc,
                IsLive = dbModel.IsLive
            };
        }

    }
}
