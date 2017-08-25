using IntoTravel.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using IntoTravel.Data.DbContextInfo;
using IntoTravel.Data.Repositories.Implementations;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Helpers;
using IntoTravel.Web.Services.Interfaces;
using IntoTravel.Web.Services.Implementations;

namespace IntoTravel.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlServerConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // db repos
            services.AddTransient<IBlogEntryRepository, BlogEntryRepository>();
            services.AddTransient<IBlogEntryPhotoRepository, BlogEntryPhotoRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IBlogEntryTagRepository, BlogEntryTagRepository>();
            services.AddTransient<ILinkRedirectionRepository, LinkRedirectionRepository>();
            services.AddTransient<IEmailSubscriptionRepository, EmailSubscriptionRepository>();
            services.AddTransient<IContentSnippetRepository, ContentSnippetRepository>();

            // db context
            services.AddTransient<IApplicationDbContext, ApplicationDbContext>();

            // other
            services.AddTransient<ICacheService, CacheService>();

            services.AddTransient<IEmailSender>(x => new AmazonMailService(
                Configuration.GetSection("AmazonEmailCredentials:AccessKey").Value, 
                Configuration.GetSection("AmazonEmailCredentials:SecretKey").Value,
                Configuration.GetSection("AmazonEmailCredentials:EmailFrom").Value));

            services.AddTransient<ISiteFilesRepository>(provider => 
                new SiteFilesRepository(Configuration.GetConnectionString("AzureStorageConnection")));
             
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
