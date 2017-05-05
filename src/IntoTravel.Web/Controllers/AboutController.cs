﻿using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IContentSnippetRepository _contentSnippetRepository;

        public AboutController(IContentSnippetRepository contentSnippetRepository)
        {
            _contentSnippetRepository = contentSnippetRepository;
        }

        public IActionResult Index()
        {
            var dbModel = _contentSnippetRepository.Get(SnippetType.About);
            var model = new ContentSnippetDisplayModel()
            {
                Content = dbModel.Content,
                SnippetType = dbModel.SnippetType
            };

            return View(model);
        }

    }
}
