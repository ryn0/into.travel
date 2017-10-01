using IntoTravel.Data.Repositories.Interfaces;
using System;
using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System.Linq;
using System.Collections.Generic;
using log4net;
using System.Reflection;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class BlogEntryTagRepository : IBlogEntryTagRepository
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BlogEntryTagRepository(IApplicationDbContext context)
        {
            Context = context;
        }

        public IApplicationDbContext Context { get; private set; }
 

        public void Dispose()
        {
            Context.Dispose();
        }

       
        public bool Delete(int tagId, int blogEntryId)
        {
            try
            {
                var entry = Context.BlogEntryTag.FirstOrDefault(x => x.TagId == tagId && x.BlogEntryId == blogEntryId);

                Context.BlogEntryTag.Remove(entry);
                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Fatal(ex);

                return false;
            }
        }

        public BlogEntryTag Create(BlogEntryTag model)
        {
            try
            {
                Context.BlogEntryTag.Add(model);
                Context.SaveChanges();

                return model;
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Update(BlogEntryTag model)
        {
            try
            {
                Context.BlogEntryTag.Update(model);
                Context.SaveChanges();

                return true;
            }

            catch (Exception ex)
            {
                log.Fatal(ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public BlogEntryTag Get(int tagId, int blogEntryId)
        {
            try
            {
                return Context.BlogEntryTag.FirstOrDefault(x => x.TagId == tagId && x.BlogEntryId == blogEntryId);
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public List<BlogEntryTag> GetTagsForBlog(int blogEntryId)
        {
            try
            {
                return Context.BlogEntryTag.Where(x => x.BlogEntryId == blogEntryId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }
    }
}
