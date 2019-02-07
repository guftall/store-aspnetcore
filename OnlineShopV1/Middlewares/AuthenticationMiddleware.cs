using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace OnlineShopV1.Middlewares
{
    public class AuthenticationMiddleware
    {

        private readonly RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,
            IAuthenticationRepository _repository,
            IAdminRepository adminRepo)
        {
            IResponse result;
            string authHeader = context.Request.Headers["authCode"];
            
            var auth = await _repository.GetByCode(authHeader);

            if (auth == null)
            {
                result = new MAuthenticationFailed();
            }
            else if (!auth.IsExpired())
            {
                context.Items.Add("auth", auth);
                /* attach admin user for use in actions
                 *
                 * this can be done in `Logout` and `Profile` actions only, to improve performance, but for now
                 * take it easy (:
                 * */
                if (auth.UserType == UserType.Admin)
                {
                    var admin = await adminRepo.GetByUsername(auth.Username);
                    context.Items.Add("admin", admin);
                }
                await _next.Invoke(context);
                return;
            }
            else
            {
                result = new MAuthenticationExpired();

            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers["Content-Type"] = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}