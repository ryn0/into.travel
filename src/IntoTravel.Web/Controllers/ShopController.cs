using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Helpers;

namespace IntoTravel.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IContentSnippetRepository _contentSnippetRepository;
        private readonly IContentSnippetHelper _contentSnippetHelper;

        public ShopController(
            IContentSnippetRepository contentSnippetRepository,
            IContentSnippetHelper contentSnippetHelper)
        {
            _contentSnippetRepository = contentSnippetRepository;
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var model = _contentSnippetHelper.GetSnippet(SnippetType.Shop);

            return View(model);
        }
    }
}
