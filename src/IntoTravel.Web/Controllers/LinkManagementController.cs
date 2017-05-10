using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Models;
using IntoTravel.Core.Utilities;
using IntoTravel.Data.Models.Db;
using System.Linq;

namespace IntoTravel.Web.Controllers
{
    [Authorize]
    public class LinkManagementController : Controller
    {
        private readonly ILinkRedirectionRepository _linkRedirectionRepository;

        public LinkManagementController(ILinkRedirectionRepository linkRedirectionRepository)
        {
            _linkRedirectionRepository = linkRedirectionRepository;
        }

        public IActionResult Index()
        {
            var allLinks = _linkRedirectionRepository.GetAll();
            var model = new LinkListModel();

            allLinks = allLinks.OrderByDescending(x => x.CreateDate).ToList();

            foreach (var link in allLinks)
            {
                model.Items.Add(new LinkEditModel()
                {
                    LinkKey = link.LinkKey,
                    LinkRedirectionId = link.LinkRedirectionId,
                    UrlDestination = link.UrlDestination
                });
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(LinkEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _linkRedirectionRepository.Create(new LinkRedirection()
            {
                LinkKey = model.LinkKey.UrlKey(),
                UrlDestination = model.UrlDestination
            });

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LinkEditModel());
        }


        [HttpPost]
        public IActionResult Edit(LinkEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var linkDbModel = _linkRedirectionRepository.Get(model.LinkRedirectionId);

            linkDbModel.LinkKey = model.LinkKey.UrlKey();
            linkDbModel.UrlDestination = model.UrlDestination.Trim();

            _linkRedirectionRepository.Update(linkDbModel);

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Edit(int linkRedirectionId)
        {
            var linkDbModel = _linkRedirectionRepository.Get(linkRedirectionId);

            var model = new LinkEditModel()
            {
                LinkKey = linkDbModel.LinkKey,
                LinkRedirectionId = linkDbModel.LinkRedirectionId,
                UrlDestination = linkDbModel.UrlDestination
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult Delete(int linkRedirectionId)
        {
            _linkRedirectionRepository.Delete(linkRedirectionId);

            return RedirectToAction("index");
        }
    }
}
