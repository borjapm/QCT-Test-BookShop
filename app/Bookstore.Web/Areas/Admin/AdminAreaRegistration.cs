using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bookstore.Web.Areas
{
    public static class AdminAreaConfiguration
    {
        public static void Configure(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}