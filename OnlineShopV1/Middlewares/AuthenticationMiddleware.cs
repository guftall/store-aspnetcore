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

        public async Task InvokeAsync(HttpContext context, IAuthenticationRepository _repository)
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
                await _next(context);
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