﻿using Microsoft.AspNetCore.Mvc;
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

        public DateTime BlogPublishDateTimeUtc { get; private set; }

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
                BlogPublishDateTimeUtc = BlogPublishDateTimeUtc,
                Key = model.Title.UrlKey()
            });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int blogEntryId)
        {
            var dbModel = _blogEntryRepository.Get(blogEntryId);

            var model = ToUiModel(dbModel);

            return View(model);
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

        [HttpGet]
        public IActionResult Preview(int blogId)
        {
            return View();
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
                    Title = entry.Title
                });
            }

            return model;
        }


        private BlogEntry ConvertToDbModel(BlogManagementEntryModel model)
        {
            return new BlogEntry()
            {
                 BlogEntryId = model.BlogEntryId,
                BlogPublishDateTimeUtc = model.BlogPublishDateTimeUtc,
                Content = model.Content,
                Title = model.Title,
            };
        }

        private static BlogManagementEntryModel ToUiModel(BlogEntry dbModel)
        {
            return new BlogManagementEntryModel()
            {
                Content = dbModel.Content,
                Title = dbModel.Title,
                BlogEntryId = dbModel.BlogEntryId,
                BlogPublishDateTimeUtc = dbModel.BlogPublishDateTimeUtc
            };
        }

    }
}
