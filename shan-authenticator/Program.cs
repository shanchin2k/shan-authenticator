#region USING

using Shan.Authentication.API.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

#endregion USING
namespace Shan.Authentication.API.Web
{
    /// <summary>
    /// The Program class which contains methods which are start of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Start of the application
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Web host builder creation to start up the application
        /// </summary>
        /// <param name="args"> The host arguments</param>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        /// <summary>
        /// Build web host for application 
        /// </summary>
        /// <param name="args"> The host arguments</param>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((webHostBuilderContext, configurationbuilder) =>
                {
                    var environment = webHostBuilderContext.HostingEnvironment;

                    configurationbuilder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
