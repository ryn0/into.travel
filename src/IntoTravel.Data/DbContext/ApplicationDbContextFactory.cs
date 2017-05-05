using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace IntoTravel.Data.DbContextInfo
{
    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public IConfigurationRoot Configuration { get; set; }


        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
             
            var builderConfigs = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

            Configuration = builderConfigs.Build();

            var connectionString = Configuration["ConnectionStrings:SqlServerConnection"];

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}
