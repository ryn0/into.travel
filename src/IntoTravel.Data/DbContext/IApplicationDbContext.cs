using IntoTravel.Data.Models;
using IntoTravel.Data.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntoTravel.Data.DbContextInfo
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<BlogEntry> BlogEntry { get; set; }

        DbSet<BlogEntryPhoto> BlogEntryPhoto { get; set; }

        DbSet<BlogEntryTag> BlogEntryTag { get; set; }

        DbSet<Tag> Tag { get; set; }

        DbSet<ApplicationUser> ApplicationUser { get; set; }

        DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }

        DbSet<LinkRedirection> LinkRedirection { get; set; }


        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
