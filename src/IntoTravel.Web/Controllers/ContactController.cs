using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Services.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ICacheService _contentSnippetHelper;

        public ContactController(
            ICacheService contentSnippetHelper)
        {
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var model = _contentSnippetHelper.GetSnippet(SnippetType.Contact);

            return View(model);
        }
    }
}
