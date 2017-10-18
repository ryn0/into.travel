using Microsoft.AspNetCore.Mvc;
using IntoTravel.Web.Helpers;
using IntoTravel.Data.Repositories.Interfaces;
using System.Text;

namespace IntoTravel.Web.Controllers
{
    public class HomeController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryRepository _blogEntryRepository;
 
        public HomeController(IBlogEntryRepository blogEntryRepository )
        {
            _blogEntryRepository = blogEntryRepository;
           
        }

        [HttpGet]
        public IActionResult Index(int pageNumber = 1)
        {
            
            int total;

            var model = ModelConverter.BlogPage(_blogEntryRepository.GetLivePage(pageNumber, AmountPerPage, out total), pageNumber, AmountPerPage, total);

            ViewData["Title"] = Data.Constants.StringConstants.DefaultPageTitle;
            ViewData["MetaDescription"] = Data.Constants.StringConstants.DefaultPageDescription;

            return View("BlogList", model);
        }

        [Route("robots.txt")]
        [HttpGet]
        public ContentResult RobotsTxt()
        {
            var sb = new StringBuilder();

            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: ");

            return Content(sb.ToString());
        }

    }
}
