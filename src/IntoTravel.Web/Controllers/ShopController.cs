using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IContentSnippetRepository _contentSnippetRepository;

        public ShopController(IContentSnippetRepository contentSnippetRepository)
        {
            _contentSnippetRepository = contentSnippetRepository;
        }

        public IActionResult Index()
        {
            var dbModel = _contentSnippetRepository.Get(SnippetType.Shop);
            var model = new ContentSnippetDisplayModel()
            {
                Content = dbModel.Content,
                SnippetType = dbModel.SnippetType
            };

            return View(model);
        }
    }
}
