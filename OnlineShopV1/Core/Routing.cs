using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OnlineShopV1.Middlewares;

namespace OnlineShopV1
{
    public static class Routing
    {
        public static void Init(IApplicationBuilder app)
        {

            app.Map("/api", appLevel1 =>
            {
                appLevel1.Map("/admin/login", appLevel2 =>
                {
                    appLevel2.UseMvc(router =>
                    {
                        router.MapRoute("Admin login", "",
                            new {controller = "Admin", action = "Login"});
                    });
                });
                appLevel1.Map("/admin/panel", appLevel2 =>
                {

                    appLevel2.UseMiddleware<AuthenticationMiddleware>();
                    appLevel2.UseMvc(router =>
                    {
                        router.MapRoute("Admin panel", "{action}",
                            new {controller = "Admin"});
                    });
                });
            });
        }
    }
}