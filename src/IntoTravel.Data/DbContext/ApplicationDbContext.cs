using IntoTravel.Data.DbModels.BaseDbModels;
using IntoTravel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntoTravel.Data.DbContextInfo
{
    public class ApplicationDbContext : ApplicationBaseContext<ApplicationDbContext>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<BlogEntry> BlogEntry { get; set; }

        public DbSet<BlogEntryPhoto> BlogEntryPhoto { get; set; }

        public DbSet<BlogEntryTag> BlogEntryTag { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BlogEntry>()
                    .HasIndex(b => b.Key)
                    .IsUnique();

            builder.Entity<BlogEntryTag>()
                .HasKey(c => new { c.BlogEntryId, c.TagId });

            builder.Entity<BlogEntryTag>()
                .HasOne(bc => bc.BlogEntry)
                .WithMany(b => b.BlogEntryTags)
                .HasForeignKey(bc => bc.BlogEntryId);

            builder.Entity<BlogEntryTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(c => c.BlogEntryTags)
                .HasForeignKey(bc => bc.TagId);

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            SetDates();

            return base.SaveChanges();
        }

        private void SetDates()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(x => (x.Entity is StateInfo)
                            && x.State == EntityState.Added)
                .Select(x => x.Entity as StateInfo))
            {
                if (entry.CreateDate == DateTime.MinValue) entry.CreateDate = DateTime.UtcNow;
            }

            foreach (var entry in ChangeTracker.Entries()
                .Where(x => (x.Entity is CreatedStateInfo)
                            && x.State == EntityState.Added)
                .Select(x => x.Entity as CreatedStateInfo))
            {
                if (entry.CreateDate == DateTime.MinValue) entry.CreateDate = DateTime.UtcNow;
            }

            foreach (var entry in ChangeTracker.Entries()
                .Where(x => x.Entity is StateInfo && x.State == EntityState.Modified)
                .Select(x => x.Entity as StateInfo))
            {
                entry.UpdateDate = DateTime.UtcNow;
            }
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SetDates();

            return base.SaveChangesAsync(cancellationToken);
        }
        
    }
}
