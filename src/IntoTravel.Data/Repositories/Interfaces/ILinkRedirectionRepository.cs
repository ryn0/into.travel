﻿using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Models.Db;
using System;
using System.Collections.Generic;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface ILinkRedirectionRepository : IDisposable
    {
        IApplicationDbContext Context { get; }

        LinkRedirection Create(LinkRedirection model);

        bool Update(LinkRedirection model);

        LinkRedirection Get(int linkRedirectionId);

        LinkRedirection Get(string key);

        List<LinkRedirection> GetAll();

        bool Delete(int linkRedirectionId);
    }
}
