using Microsoft.AspNetCore.Mvc;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class EmailSubscriptionManagementController : Controller
    {
        private readonly IEmailSubscriptionRepository _emailSubscriptionRepository;

        public EmailSubscriptionManagementController(IEmailSubscriptionRepository emailSubscriptionRepository)
        {
            _emailSubscriptionRepository = emailSubscriptionRepository;
        }

        public IActionResult Index()
        {
            var allEmail = _emailSubscriptionRepository.GetAll();
            var model = new EmailSubscribeEditListModel();

            foreach (var sub in allEmail)
            {
                model.Items.Add(new EmailSubscribeEditModel()
                {
                    Email = sub.Email,
                    IsSubscribed = sub.IsSubscribed,
                    EmailSubscriptionId = sub.EmailSubscriptionId
                });
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(EmailSubscribeEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dbModel = _emailSubscriptionRepository.Get(model.EmailSubscriptionId);

            dbModel.Email = model.Email;
            dbModel.IsSubscribed = model.IsSubscribed;

            _emailSubscriptionRepository.Update(dbModel);

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Edit(int emailSubscriptionId)
        {
            var dbModel = _emailSubscriptionRepository.Get(emailSubscriptionId);

            var model = new EmailSubscribeEditModel()
            {
                Email = dbModel.Email,
                IsSubscribed = dbModel.IsSubscribed,
                EmailSubscriptionId = dbModel.EmailSubscriptionId
            };

            return View(model);
        }
    }
}