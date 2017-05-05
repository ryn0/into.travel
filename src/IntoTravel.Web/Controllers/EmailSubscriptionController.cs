using System;
using Microsoft.AspNetCore.Mvc;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;

namespace IntoTravel.Web.Controllers
{
    public class EmailSubscriptionController : Controller
    {
        private readonly IEmailSubscriptionRepository _emailSubscriptionRepository;

        public EmailSubscriptionController(IEmailSubscriptionRepository emailSubscriptionRepository)
        {
            _emailSubscriptionRepository = emailSubscriptionRepository;
        }

        [HttpPost]
        public IActionResult Subscribe(EmailSubscribeModel model)
        {
            if (!ModelState.IsValid)
                throw new Exception("invalid email submission");

            var emailDbModel = _emailSubscriptionRepository.Get(model.Email);

            if (emailDbModel == null || emailDbModel.EmailSubscriptionId == 0)
            {
                _emailSubscriptionRepository.Create(new Data.Models.Db.EmailSubscription()
                {
                    Email = model.Email,
                    IsSubscribed = true
                });
            }

            return View();
        }
    }
}
