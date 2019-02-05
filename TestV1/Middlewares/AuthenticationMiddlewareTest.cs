using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using OnlineShopV1;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;
using OnlineShopV1.Middlewares;
using TestV1.Models;
using Xunit;
using StatusCodes = OnlineShopV1.Core.Responses.StatusCodes;

namespace TestV1
{
    public class AuthenticationMiddlewareTest
    {
        private const string authCode = "fake_auth_code123";
        
        [Fact]
        public async Task EmptyAuthCode_ReturnAuthenticationFailed()
        {

            var mockRepo = new Mock<IAuthenticationRepository>();
            var context = new DefaultHttpContext();
            
            var middleware = new AuthenticationMiddleware(httpContext => throw new Exception());

            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, mockRepo.Object);


            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var streamText = new StreamReader(context.Response.Body).ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<DefaultResponse>(streamText);

            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationFailed, objResponse.status);
        }
        
        [Fact]
        public async Task RandomAuthCode_ReturnAuthenticationFailed()
        {

            var mockRepo = new Mock<IAuthenticationRepository>();
            var context = new DefaultHttpContext();
            
            var middleware = new AuthenticationMiddleware(httpContext => throw new Exception());

            context.Response.Body = new MemoryStream();
            context.Request.Headers["authCode"] = "some_random_string";

            await middleware.InvokeAsync(context, mockRepo.Object);


            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var streamText = new StreamReader(context.Response.Body).ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<DefaultResponse>(streamText);

            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationFailed, objResponse.status);
        }
        
        [Fact]
        public async Task ExpiredAuthentication_ReturnAuthenticationExpired()
        {

            var auth = AuthenticationTest.GetTestAuth();
            auth.Code = authCode;
            auth.Expires = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));
            
            var mockRepo = new Mock<IAuthenticationRepository>();
            mockRepo.Setup(r => r.GetByCode(authCode))
                .ReturnsAsync(auth);
            
            var context = new DefaultHttpContext();
            
            var middleware = new AuthenticationMiddleware(httpContext => throw new Exception());

            context.Response.Body = new MemoryStream();
            context.Request.Headers["authCode"] = authCode;

            await middleware.InvokeAsync(context, mockRepo.Object);


            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var streamText = new StreamReader(context.Response.Body).ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<DefaultResponse>(streamText);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationExpired, objResponse.status);
        }
        
        
        [Fact]
        public async Task ValidAuthentication_PassRequestToNextRDInPipeline()
        {

            var auth = AuthenticationTest.GetTestAuth();
            auth.Code = authCode;
            auth.Expires = DateTime.Now.Add(TimeSpan.FromDays(1));
            const int customStatusCode = 123;
            
            var mockRepo = new Mock<IAuthenticationRepository>();
            mockRepo.Setup(r => r.GetByCode(authCode))
                .ReturnsAsync(auth);
            
            var context = new DefaultHttpContext();
            
            var middleware = new AuthenticationMiddleware(httpContext =>
            {
                httpContext.Response.StatusCode = customStatusCode;
                return Task.CompletedTask;
            });

            context.Response.Body = new MemoryStream();
            context.Request.Headers["authCode"] = authCode;

            await middleware.InvokeAsync(context, mockRepo.Object);

            Assert.Equal(customStatusCode, context.Response.StatusCode);
        }
    }
}