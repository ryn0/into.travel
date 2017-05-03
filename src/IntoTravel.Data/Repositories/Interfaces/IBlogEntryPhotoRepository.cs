using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface IBlogEntryPhotoRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        BlogEntryPhoto Create(BlogEntryPhoto model);

        bool Update(BlogEntryPhoto model);

        BlogEntryPhoto Get(int blogEntryPhotoId);

        bool Delete(int blogEntryPhotoId);

        List<BlogEntryPhoto> GetBlogPhotos(int blogEntryId);

        void SetDefaultPhoto(int blogEntryPhotoId);
    }
}
