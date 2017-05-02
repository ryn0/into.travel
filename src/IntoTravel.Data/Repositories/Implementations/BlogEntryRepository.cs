﻿using IntoTravel.Data.Repositories.Interfaces;
using System;
using IntoTravel.Data.Models;
using IntoTravel.Data.DbContextInfo;
using System.Linq;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class BlogEntryRepository : IBlogEntryRepository
    {
        public BlogEntryRepository(IApplicationDbContext context)
        {
            Context = context;
        }

        public IApplicationDbContext Context { get; private set; }
 
        public BlogEntry Create(BlogEntry model)
        {
            try
            {
                Context.BlogEntry.Add(model);
                Context.SaveChanges();

                return model;
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public List<BlogEntry> GetPage(int pageNumber , int quantityPerPage, out int total)
        {
            try
            {
                var model = Context.BlogEntry
                    .OrderByDescending(blog => blog.CreateDate)
                    .Skip(quantityPerPage * (pageNumber - 1))
                    .Take(quantityPerPage)
                    .ToList();

                total = Context.BlogEntry.Count();
             
                return model;
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                throw new Exception("DB error", ex.InnerException);
            }
  
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public BlogEntry Get(int blogEntryId)
        {
            try
            {
                return Context.BlogEntry
                              .FirstOrDefault(x => x.BlogEntryId == blogEntryId);
            }
            catch (Exception ex)
            {
                throw new Exception("DB error", ex.InnerException);
            }
        }

        public BlogEntry Get(string key)
        {
            try
            {
                return Context.BlogEntry
                              .FirstOrDefault(x => x.Key == key);
            }
            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Update(BlogEntry model)
        {
            try
            {
                Context.BlogEntry.Update(model);
                Context.SaveChanges();

                return true;
            }

            catch (Exception ex)
            {
                // Log.Fatal(LogCodes.SqlError, ex);
                throw new Exception("DB error", ex.InnerException);

            }
        }

        public bool Delete(int blogEntryId)
        {
            try
            {
                var entry = Context.BlogEntry
                              .FirstOrDefault(x => x.BlogEntryId == blogEntryId);

                Context.BlogEntry.Remove(entry);
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