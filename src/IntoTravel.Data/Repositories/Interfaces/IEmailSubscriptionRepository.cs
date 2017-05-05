using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models.Db;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface IEmailSubscriptionRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        EmailSubscription Create(EmailSubscription model);

        bool Update(EmailSubscription model);

        EmailSubscription Get(int emailSubscriptionId);

        EmailSubscription Get(string email);

        List<EmailSubscription> GetAll();

        bool Delete(int emailSubscriptionId);
    }
}
