using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Services.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly ICacheService _contentSnippetHelper;

        public AboutController(
            ICacheService contentSnippetHelper)
        {
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var model = _contentSnippetHelper.GetSnippet(SnippetType.About);

            return View(model);
        }
    }
}
