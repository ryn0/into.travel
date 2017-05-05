using IntoTravel.Data.Repositories.Interfaces;
using System;
using IntoTravel.Data.DbContextInfo;
using System.Linq;
using IntoTravel.Data.Enums;
using IntoTravel.Data.Models.Db;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class ContentSnippetRepository : IContentSnippetRepository
    {
        public ContentSnippetRepository(IApplicationDbContext context)
        {
            Context = context;
        }

        public IApplicationDbContext Context { get; private set; }

        public ContentSnippet Create(ContentSnippet model)
        {
            try
            {
                Context.ContentSnippet.Add(model);
                Context.SaveChanges();

                return model;
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

        public ContentSnippet Get(int contentSnippetId)
        {
            try
            {
                return Context.ContentSnippet.FirstOrDefault(x => x.ContentSnippetId == contentSnippetId);
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public ContentSnippet Get(SnippetType snippetType)
        {
            try
            {
                return Context.ContentSnippet.FirstOrDefault(x => x.SnippetType == snippetType);
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Update(ContentSnippet model)
        {
            try
            {
                Context.ContentSnippet.Update(model);
                Context.SaveChanges();

                return true;
            }

            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Delete(int contentSnippetId)
        {
            try
            {
                var entry = Context.ContentSnippet.FirstOrDefault(x => x.ContentSnippetId == contentSnippetId);

                Context.ContentSnippet.Remove(entry);
                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);

                return false;
            }
        }
         
        public List<ContentSnippet> GetAll()
        {
            try
            {
                return Context.ContentSnippet.ToList();
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);
            }
        }
    }
}
