using Microsoft.AspNetCore.Mvc;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Enums;
using IntoTravel.Web.Helpers;

namespace IntoTravel.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IContentSnippetRepository _contentSnippetRepository;
        private readonly IContentSnippetHelper _contentSnippetHelper;

        public AboutController(
            IContentSnippetRepository contentSnippetRepository,
            IContentSnippetHelper contentSnippetHelper)
        {
            _contentSnippetRepository = contentSnippetRepository;
            _contentSnippetHelper = contentSnippetHelper;
        }

        public IActionResult Index()
        {
            var model = _contentSnippetHelper.GetSnippet(SnippetType.About);

            return View(model);
        }
    }
}
