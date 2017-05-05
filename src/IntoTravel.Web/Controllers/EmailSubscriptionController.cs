using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Controllers
{
    public class EmailSubscriptionController : Controller
    {
        public EmailSubscriptionController()
        {

        }

        public IActionResult Subscribe(EmailSubscribeModel model)
        {
            return View();
        }
    }
}
