using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Services.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ICacheService _contentSnippetHelper;

        public ShopController(
            ICacheService contentSnippetHelper)
        {
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var model = _contentSnippetHelper.GetSnippet(SnippetType.Shop);

            return View(model);
        }
    }
}
