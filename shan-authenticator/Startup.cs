#region USING

using Shan.Authentication.API.Common;
using Shan.Authentication.API.Entities;
using Shan.Authentication.API.Web.ApiHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion USING

namespace Shan.Authentication.API.Web
{
    /// <summary>
    /// The Startup class which contains services and middle ware to be registered in application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration configuration { get; }

        /// <summary>
        /// To hold the startup exception
        /// </summary>
        public Exception StartupException;

        /// <summary>
        /// Application settings - initialization
        /// </summary>
        AppSettings appSettings = new AppSettings();

        /// <summary>
        /// Start up Constructor
        /// </summary>        
        /// <param name="hostingEnvironment"> Hosting environment</param>
        public Startup(IHostEnvironment hostingEnvironment)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(hostingEnvironment.ContentRootPath);
                builder.AddJsonFile(DataConstant.AppSettingsFileName, false, true);
                builder.AddJsonFile(DataConstant.ResourceFileName, false, true);
                builder.AddEnvironmentVariables();
                configuration = builder.Build();
            }
            catch (Exception exception)
            {
                StartupException = exception;
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> The Service collection</param>        
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable logging from authentication flow
            IdentityModelEventSource.ShowPII = true;

            // Any exception occurred in Startup constructor then return to Configure method
            // there application will return with custom exception response.
            if (StartupException != null)
            {
                return;
            }

            try
            {
                services.AddMvcCore(filter =>
                {
                    filter.Filters.Add(typeof(ExceptionAttribute));
                });

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                // To configure application settings to service      
                services.Configure<AppSettings>(configuration.GetSection(DataConstant.Appsettings));
                configuration.Bind(DataConstant.Appsettings, appSettings);
                configuration.Bind(DataConstant.ApplicationInsights, appSettings.ApplicationInsights);

                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddAzureAdB2C(options => configuration.Bind(DataConstant.AppSettingsAzureAdB2C, options))
                .AddCookie();

                services.AddSingleton(appSettings);
                services.AddSingleton(configuration);
                services.AddSingleton(appSettings.AzureAdB2C);
                services.AddSingleton(appSettings.ApplicationInsights);

                // Add framework services
                // Contract resolver is initialized since default resolver is set as CamelCasePropertyNamesContractResolver by .Net core if not initialized
                services.AddMvc()
                    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

                // Adds a default in-memory implementation of IDistributedCache.
                services.AddDistributedMemoryCache();
                services.AddSession();

                // Add application settings helper to service in order to access values.
                ConfigHelper configHelper = new ConfigHelper(appSettings);
                services.AddSingleton(typeof(ConfigHelper));

                // Add Api Response Helper to service to use configuration.
                ApiResponseHelper Helper = new ApiResponseHelper(configuration);
                services.AddSingleton(typeof(ApiResponseHelper));
                services.AddHttpContextAccessor();
            }
            catch (Exception exception)
            {
                StartupException = exception;
            }
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"> The application builder</param>
        /// <param name="env"> The host environment</param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // If any exception throws during startup, handle those
            if (StartupException != null)
            {
                app.Run(async (context) =>
                {
                    await ConstructStartupException(context, StartupException);
                });
                return;
            }

            if (env.IsDevelopment())
            {
                // Middle ware invoked to catch exceptions.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Default middleware to catch the un-handled exceptions occurred before reaching the controller code
                app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = feature.Error;

                        await ConstructStartupException(context, exception);
                    });
                });

            }

            // Adds MVC to the IApplicationBuilder request execution pipeline.
            app.UseStaticFiles();

            // Use Session to maintain context across redirects
            app.UseSession();

            // Use Authentication
            app.UseAuthentication();

            // Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            app.UseRouting();

            // Adds Cors policy to allow only white-listed methods
            app.UseCors(option => option.WithMethods(DataConstant.HttpGET));

            // Executes the endpoint that was selected by routing.
            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// To construct and return the startup exception response
        /// </summary>
        /// <param name="context"> The request context</param>
        /// <param name="exception"> The Exception </param>
        /// <returns> Startup exception response object</returns>
        private static async Task ConstructStartupException(HttpContext context, Exception exception)
        {
            ApiResponse apiResponse = new ApiResponse
            {
                ResponseCode = DataConstant.Authentication003,
                ResponseMessage = DataConstant.SystemStartupExceptionMessage,
                StatusCode = HttpStatusCode.InternalServerError
            };

            // Log the un-handled exception
            DiagnosticLogManager.TrackException(exception);

            // Log the trace information
            DiagnosticLogManager.TrackTrace(Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error, context.Request.HttpContext, apiResponse);

            var result = JsonConvert.SerializeObject(apiResponse);
            context.Response.StatusCode = 500;
            context.Response.ContentType = DataConstant.MediaTypeHeaderApplicationJson;
            await context.Response.WriteAsync(result);
        }

    }
}
