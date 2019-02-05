using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OnlineShopV1.Middlewares;

namespace OnlineShopV1
{
    public static class Routing
    {

        public static void Init(IApplicationBuilder app)
        {
            app.Map("/api", (appLevel1) =>
            {

                appLevel1.UseMvc(router =>
                {
                    router.MapRoute("Admin login", "admin/login",
                        new {controller = "Admin", action = "Login"});
                });
                appLevel1.Map("/admin/panel", appLevel3 =>
                {
                    /////////
                    appLevel3.UseMiddleware<AuthenticationMiddleware>();
                    ////////////
                });
                
                appLevel1.UseMvc(); 
            });
        }
    }
}