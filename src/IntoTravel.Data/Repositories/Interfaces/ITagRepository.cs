using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface ITagRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        Tag Create(Tag model);

        bool Update(Tag model);

        Tag Get(int tagId);

        Tag Get(string name);

        bool Delete(int tagId);
    }
}
