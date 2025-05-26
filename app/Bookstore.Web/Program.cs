
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
using System.Data.Entity;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add settings from web.config appSettings
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ClientValidationEnabled"] = "true",
                    ["Environment"] = "Development",
                    ["Services:Authentication"] = "local",
                    ["Services:Database"] = "local",
                    ["Services:FileService"] = "local",
                    ["Services:ImageValidationService"] = "local",
                    ["Services:LoggingService"] = "local",
                    ["Authentication:Cognito:LocalClientId"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']",
                    ["Authentication:Cognito:AppRunnerClientId"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']",
                    ["Authentication:Cognito:MetadataAddress"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']",
                    ["Authentication:Cognito:CognitoDomain"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']",
                    ["Files:BucketName"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']",
                    ["Files:CloudFrontDomain"] = "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']"
                });

                // Add connection string for Entity Framework 6
                builder.Services.AddScoped<DbContext>(provider =>
                {
                    // Using the connection string from web.config
                    var connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";
                    // Create and return your EF6 DbContext instance
                    return new DbContext(connectionString);
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register areas
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // Add global filters equivalent to FilterConfig.RegisterGlobalFilters
                    });

                // AddOptimization is replaced by built-in static file handling
                //Added Services

                var app = builder.Build();
                
                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                
                //Added Middleware

                app.UseRouting();

                app.UseAuthorization();

                // Global error logger (replacing Application_Error in Global.asax.cs)
                app.Use(async (context, next) => {
                    try
                    {
                        await next(context);
                    }
                    catch (Exception ex)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Unhandled exception");
                        throw;
                    }
                });
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Register additional routes (equivalent to RouteConfig.RegisterRoutes)

                // Register area routes
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Set configuration values at startup
                var environment = app.Configuration["Environment"] ?? "Development";
                var authType = app.Configuration["Services:Authentication"] ?? "local";
                var dbType = app.Configuration["Services:Database"] ?? "local";
                var fileServiceType = app.Configuration["Services:FileService"] ?? "local";

                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }