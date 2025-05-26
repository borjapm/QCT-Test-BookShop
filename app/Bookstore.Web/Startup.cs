using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;



[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure logging directly
            ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
    }

    private void ConfigureLogging()
    {
        // Implement logging configuration here
        // This is a simplified implementation
        try
        {
            // Add your logging configuration code here
// For example, if using NLog:
            // NLog.LogManager.LoadConfiguration("NLog.config");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Error initializing logging: {ex.Message}");
        }
    }
}

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Implementation for configuration setup
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Implementation for dependency injection setup
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Implementation for authentication setup
        }
    }
}