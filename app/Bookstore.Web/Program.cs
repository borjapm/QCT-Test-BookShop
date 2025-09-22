
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Data.Entity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using WebOptimizer;

namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add connection strings from Web.config
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            // Store configuration in static ConfigurationManager
            ConfigurationManager.Configuration = builder.Configuration;

            // Add services to the container (formerly ConfigureServices)
            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // Configure client validation
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                    (x) => $"The value '{x}' is invalid.");
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
                    (x) => $"The field must be a number.");
            });

            // Register areas
            builder.Services.AddMvc()
                .AddMvcOptions(options =>
                {
                    // Add any global filters here if needed
                });

            // Add bundling and minification support
            builder.Services.AddWebOptimizer(pipeline =>
            {
                // Configure bundles here
            });

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add service configurations from appSettings
            var environment = builder.Configuration["Environment"] ?? "Development";
            var authService = builder.Configuration["Services/Authentication"] ?? "local";
            var dbService = builder.Configuration["Services/Database"] ?? "local";
            var fileService = builder.Configuration["Services/FileService"] ?? "local";
            var imageValidationService = builder.Configuration["Services/ImageValidationService"] ?? "local";
            var loggingService = builder.Configuration["Services/LoggingService"] ?? "local";

            // Configure EntityFramework
            if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("BookstoreDatabaseConnection")))
            {
                // EntityFramework 6 configuration
                Database.SetInitializer<DbContext>(null); // No database initializer by default
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline (formerly Configure method)
            if (app.Environment.IsDevelopment() || environment == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Register global error handler
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    logger.LogError(exception, "Unhandled exception");

                    await context.Response.WriteAsync("An error occurred. Please try again later.");
                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseWebOptimizer(); // For bundling and minification

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }

    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }
    }
}
