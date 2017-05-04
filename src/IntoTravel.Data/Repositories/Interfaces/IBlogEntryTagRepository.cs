using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface IBlogEntryTagRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        BlogEntryTag Create(BlogEntryTag model);

        bool Update(BlogEntryTag model);

        BlogEntryTag Get(int tagId, int blogEntryId);

        List<BlogEntryTag> GetTagsForBlog(int blogEntryId);

        bool Delete(int tagId, int blogEntryId);
    }
}
