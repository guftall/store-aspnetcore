using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using OnlineShopV1;
using OnlineShopV1.Core.Responses;
using OnlineShopV1.Middlewares;
using TestV1.Models;
using Xunit;
using StatusCodes = OnlineShopV1.Core.Responses.StatusCodes;
using TestV1.Controllers;

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

            await middleware.Invoke(context, mockRepo.Object, null);


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

            await middleware.Invoke(context, mockRepo.Object, null);


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

            await middleware.Invoke(context, mockRepo.Object, null);


            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var streamText = new StreamReader(context.Response.Body).ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<DefaultResponse>(streamText);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationExpired, objResponse.status);
        }
        
        
        [Fact]
        public async Task ValidAuthentication_PopulateAuth_PassRequestToNextRDInPipeline()
        {

            var auth = AuthenticationTest.GetTestAuth();
            auth.Code = authCode;
            auth.UserType = UserType.Normal;
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

            await middleware.Invoke(context, mockRepo.Object, null);

            Assert.Equal(customStatusCode, context.Response.StatusCode);
            Assert.Equal(auth, context.Items["auth"] as Authentication);
        }

        [Fact]
        public async Task ValidAuthentication_PopulateAdminFromDb_ContinuePipeline()
        {

            var auth = AuthenticationTest.GetTestAuth();
            auth.Code = authCode;
            auth.UserType = UserType.Admin;
            auth.Expires = DateTime.Now.Add(TimeSpan.FromDays(1));
            const int customStatusCode = 123;
            
            var mockRepo = new Mock<IAuthenticationRepository>();
            mockRepo.Setup(r => r.GetByCode(authCode))
                .ReturnsAsync(auth);
            
            AdminControllerTest.Init();
            auth.Username = AdminControllerTest.tmpAdmin.Username;
            
            var mockAdminRepo = new Mock<IAdminRepository>();
            mockAdminRepo.Setup(r => r.GetByUsername(auth.Username))
                .ReturnsAsync(AdminControllerTest.tmpAdmin);
            
            var context = new DefaultHttpContext();
            
            var middleware = new AuthenticationMiddleware(httpContext =>
            {
                httpContext.Response.StatusCode = customStatusCode;
                return Task.CompletedTask;
            });

            context.Response.Body = new MemoryStream();
            context.Request.Headers["authCode"] = authCode;

            await middleware.Invoke(context, mockRepo.Object, mockAdminRepo.Object);

            Assert.Equal(customStatusCode, context.Response.StatusCode);
            Assert.Equal(AdminControllerTest.tmpAdmin, context.Items["admin"] as Admin);
        }
    }
}