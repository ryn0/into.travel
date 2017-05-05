using Microsoft.AspNetCore.Mvc;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Models.Db;

namespace IntoTravel.Web.Controllers
{
    public class EmailSubscriptionManagementController : Controller
    {
        private readonly IEmailSubscriptionRepository _emailSubscriptionRepository;

        public EmailSubscriptionManagementController(IEmailSubscriptionRepository emailSubscriptionRepository)
        {
            _emailSubscriptionRepository = emailSubscriptionRepository;
        }

        public IActionResult Subscribe(EmailSubscribeModel model)
        {
            if (!ModelState.IsValid)
                throw new System.Exception();

            var existingEmail = _emailSubscriptionRepository.Get(model.Email);

            if (existingEmail == null || existingEmail.EmailSubscriptionId == 0)
            {
                _emailSubscriptionRepository.Create(new EmailSubscription()
                {
                    Email = model.Email,
                    IsSubscribed = true
                });
            }

            return View();
        }
    }
}
