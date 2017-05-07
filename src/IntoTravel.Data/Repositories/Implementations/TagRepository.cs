using IntoTravel.Data.Repositories.Interfaces;
using System;
using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System.Linq;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        public TagRepository(IApplicationDbContext context)
        {
            Context = context;
        }

        public IApplicationDbContext Context { get; private set; }

        public Tag Create(Tag model)
        {
            try
            {
                Context.Tag.Add(model);
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

        public Tag Get(int tagId)
        {
            try
            {
                return Context.Tag.FirstOrDefault(x => x.TagId == tagId);
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public Tag Get(string key)
        {
            try
            {
                return Context.Tag.FirstOrDefault(x => x.Key == key);
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Update(Tag model)
        {
            try
            {
                Context.Tag.Update(model);
                Context.SaveChanges();

                return true;
            }

            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Delete(int tagId)
        {
            try
            {
                var entry = Context.Tag.FirstOrDefault(x => x.TagId == tagId);

                Context.Tag.Remove(entry);
                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);

                return false;
            }
        }
    }
}
