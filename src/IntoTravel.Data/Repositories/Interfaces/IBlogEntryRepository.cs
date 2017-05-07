using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface IBlogEntryRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        BlogEntry Create(BlogEntry model);

        bool Update(BlogEntry model);

        BlogEntry Get(int blogEntryId);

        bool Delete(int blogEntryId);

        BlogEntry Get(string key);

        List<BlogEntry> GetPage(int pageNumber , int quantityPerPage, out int total);

        List<BlogEntry> GetLivePage(int pageNumber, int quantityPerPage, out int total);

        List<BlogEntry> GetLivePageByTag(string tagKey, int pageNumber, int quantityPerPage, out int total);
    }
}
