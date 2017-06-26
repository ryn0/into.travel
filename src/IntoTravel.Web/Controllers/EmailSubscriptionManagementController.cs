using Microsoft.AspNetCore.Mvc;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;
using System.Text;

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
            var allEmails = _emailSubscriptionRepository.GetAll();
            var model = new EmailSubscribeEditListModel();

            foreach (var sub in allEmails)
            {
                model.Items.Add(new EmailSubscribeEditModel()
                {
                    Email = sub.Email,
                    IsSubscribed = sub.IsSubscribed,
                    EmailSubscriptionId = sub.EmailSubscriptionId
                });
            }

            if (allEmails != null && allEmails.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var sub in allEmails)
                {
                    if (!sub.IsSubscribed)
                        continue;

                    sb.AppendFormat("{0}, ", sub.Email);
                }

                model.Emails = sb.ToString();
                model.Emails = model.Emails.Trim().TrimEnd(',');
            }

            var link = string.Format("{0}://{1}/EmailSubscription/Unsubscribe", HttpContext.Request.Scheme, HttpContext.Request.Host);
            model.UnsubscribeLink = link;

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