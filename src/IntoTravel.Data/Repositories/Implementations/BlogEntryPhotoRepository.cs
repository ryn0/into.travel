using IntoTravel.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System.Linq;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class BlogEntryPhotoRepository : IBlogEntryPhotoRepository
    {
        public IApplicationDbContext Context { get; set; }

        public BlogEntryPhotoRepository(IApplicationDbContext context)
        {
            Context = context;
        }

        public BlogEntryPhoto Create(BlogEntryPhoto model)
        {
            try
            {
                Context.BlogEntryPhoto.Add(model);
                Context.SaveChanges();

                return model;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public void SetDefaultPhoto(int blogEntryPhotoId)
        {
            try
            {
                var photoEntry = Get(blogEntryPhotoId);

                foreach (var photo in Context.BlogEntryPhoto
                                             .Where(x => x.BlogEntryId == photoEntry.BlogEntryId)
                                             .ToList())
                {
                    photo.IsDefault = false;

                    if (photo.BlogEntryPhotoId == blogEntryPhotoId)
                    {
                        photo.IsDefault = true;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public bool Delete(int blogEntryPhotoId)
        {
            try
            {
                var entry = Context.BlogEntryPhoto
                                   .FirstOrDefault(x => x.BlogEntryPhotoId == blogEntryPhotoId);

                Context.BlogEntryPhoto.Remove(entry);
                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);

                return false;
            }
        }
 
        public BlogEntryPhoto Get(int blogEntryPhotoId)
        {
            try
            {
                return Context.BlogEntryPhoto
                              .FirstOrDefault(x => x.BlogEntryPhotoId == blogEntryPhotoId);
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public List<BlogEntryPhoto> GetBlogPhotos(int blogEntryId)
        {
            try
            {
                return Context.BlogEntryPhoto
                              .Where(x => x.BlogEntryId == blogEntryId)
                              .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public bool Update(BlogEntryPhoto model)
        {
            try
            {
                Context.BlogEntryPhoto.Update(model);
                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }
      
        public void Dispose()
        {
            Context.Dispose();
        }
  
        
    }
}
