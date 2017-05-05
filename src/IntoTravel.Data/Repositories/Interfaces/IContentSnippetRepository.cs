using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Enums;
using IntoTravel.Data.Models.Db;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface IContentSnippetRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        ContentSnippet Create(ContentSnippet model);

        bool Update(ContentSnippet model);

        ContentSnippet Get(int contentSnippetId);

        ContentSnippet Get(SnippetType snippetType);

        bool Delete(int tagId);

        List<ContentSnippet> GetAll();
    }
}
