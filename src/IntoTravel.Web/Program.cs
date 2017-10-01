using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
// using log4net.Config;
using System.Reflection;
// using log4net;

namespace IntoTravel.Web
{
    public class Program
    {
        // private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            // var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            // XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // log.Info("Application - Main is invoked");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
