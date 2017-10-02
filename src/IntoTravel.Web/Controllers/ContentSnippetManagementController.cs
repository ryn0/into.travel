using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Models;
using IntoTravel.Data.Models.Db;
using System.Linq;
using IntoTravel.Services.Interfaces;

namespace IntoTravel.Web.Controllers
{
    [Authorize]
    public class ContentSnippetManagementController : Controller
    {
        private readonly IContentSnippetRepository _contentSnippetRepository;
        private readonly ICacheService _contentSnippetHelper;

        public ContentSnippetManagementController(
            IContentSnippetRepository contentSnippetRepository, 
            ICacheService contentSnippetHelper)
        {
            _contentSnippetRepository = contentSnippetRepository;
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var allSnippets = _contentSnippetRepository.GetAll().OrderBy(x => x.SnippetType.ToString());
            var model = new ContentSnippetEditListModel();

            foreach (var snippet in allSnippets)
            {
                model.Items.Add(new ContentSnippetEditModel()
                {
                    Content = snippet.Content,
                    ContentSnippetId = snippet.ContentSnippetId,
                    SnippetType = snippet.SnippetType
                });
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ContentSnippetEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _contentSnippetRepository.Create(new ContentSnippet()
            {
                Content = model.Content,
                ContentSnippetId = model.ContentSnippetId,
                SnippetType = model.SnippetType
            });

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ContentSnippetEditModel()
            {
                
            };
                
            return View();
        }


        [HttpPost]
        public IActionResult Edit(ContentSnippetEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dbModel = _contentSnippetRepository.Get(model.ContentSnippetId);

            dbModel.Content = model.Content;
            dbModel.SnippetType = model.SnippetType;

            _contentSnippetRepository.Update(dbModel);

            _contentSnippetHelper.ClearSnippetCache(model.SnippetType);

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Edit(int contentSnippetId)
        {
            var dbModel = _contentSnippetRepository.Get(contentSnippetId);

            var model = new ContentSnippetEditModel()
            {
                Content = dbModel.Content,
                ContentSnippetId = dbModel.ContentSnippetId,
                SnippetType = dbModel.SnippetType,
                
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult Delete(int contentSnippetId)
        {
            _contentSnippetRepository.Delete(contentSnippetId);

            return RedirectToAction("index");
        }
    }
}
