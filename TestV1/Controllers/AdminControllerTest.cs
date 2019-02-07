using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShopV1;
using OnlineShopV1.Controllers;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;
using Xunit;
using StatusCodes = OnlineShopV1.Core.Responses.StatusCodes;

namespace TestV1
{
    public class AdminControllerTest
    {

        public static Mock<IAuthenticationRepository> authRepo;
        public static  Mock<IAdminRepository> adminRepo;
        public static  Mock<IOptionsMonitor<GlobalOptions>> opts;
        public static  Admin tmpAdmin;
        public static string AuthCode = "some_random_auth_code";

        [Fact]
        public async Task LoginAdminNotFoundUsername_ReturnUnAuthorized()
        {

            Init();
            
            adminRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync((Admin) null);
            var controller = GetController();

            var actionResult = await controller.Login(tmpAdmin);

            var objectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var response = Assert.IsType<MAuthenticationFailed>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationFailed, response.status);

        }

        [Fact]
        public async Task LoginWrongPassword_ReturnUnAuthorized()
        {
            Init();
            adminRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync((Admin) null);
            var controller = GetController();

            var actionResult = await controller.Login(tmpAdmin);
            var objectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var response = Assert.IsType<MAuthenticationFailed>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.AuthenticationFailed, response.status);
        }

        [Fact]
        public async Task LoginSuccess_AddNewAuthentication_ReturnAuthenticationCode()
        {
            Init();

            tmpAdmin.HashPassword();
            var dbAdmin = new Admin
            {
                Password = tmpAdmin.Password,
                Username = tmpAdmin.Username
            };
            dbAdmin.HashPassword();
            
            adminRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync(dbAdmin);
            
            authRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync((Authentication) null);

            authRepo.Setup(r => r.AddAuthentication(null))
                .Returns(Task.CompletedTask);
            var controller = GetController();

            var actionResult = await controller.Login(tmpAdmin);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.Success, response.status);
            Assert.False(string.IsNullOrEmpty(response.AuthCode));
        }

        [Fact]
        public async Task LoginSuccess_UpdateExpiredAuthentication_ReturnAuthenticationCode()
        {
            Init();
            var tmpAuth = new Authentication
            {
                Code = AuthCode,
                Username = tmpAdmin.Username,
                Expires = DateTime.Now.Subtract(TimeSpan.FromSeconds(1)),
                UserType = UserType.Admin
            };
            var dbAdmin = new Admin
            {
                Password = tmpAdmin.Password,
                Username = tmpAdmin.Username
            };
            dbAdmin.HashPassword();
            
            adminRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync(dbAdmin);
            
            authRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync(tmpAuth);

            authRepo.Setup(r => r.UpdateAuthentication(tmpAuth))
                .Returns(Task.CompletedTask);
            
            var controller = GetController();

            var actionResult = await controller.Login(tmpAdmin);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.Success, response.status);
            Assert.False(string.IsNullOrEmpty(response.AuthCode));
            Assert.NotEqual(AuthCode, response.AuthCode);
        }
        
        [Fact]
        public async Task LoginSuccess_ReturnAuthenticationCode()
        {
            Init();
            var tmpAuth = new Authentication
            {
                Code = AuthCode,
                Username = tmpAdmin.Username,
                Expires = DateTime.Now.AddMinutes(2),
                UserType = UserType.Admin
            };
            var dbAdmin = new Admin
            {
                Username = tmpAdmin.Username,
                Password = tmpAdmin.Password
            };
            dbAdmin.HashPassword();
            
            adminRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync(dbAdmin);
            
            authRepo.Setup(r => r.GetByUsername(tmpAdmin.Username))
                .ReturnsAsync(tmpAuth);
            
            var controller = GetController();

            var actionResult = await controller.Login(tmpAdmin);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.Success, response.status);
            Assert.False(string.IsNullOrEmpty(response.AuthCode));
            Assert.Equal(tmpAuth.Code, response.AuthCode);
        }

        [Fact]
        public async Task LogoutSuccess_ReturnLoggedOutResponse()
        {
            Init();

            var controller = GetController();

            var tmpAuth = new Authentication
            {
                ID = tmpAdmin.ID,
                Username = tmpAdmin.Username
            };

            authRepo.Setup(r => r.Remove(tmpAuth))
                .Returns(Task.CompletedTask);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Items.Add("admin", tmpAdmin);

            var actionResult = await controller.Logout();
            
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<DefaultResponse>(objectResult.Value);
            
            Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.Equal((int) StatusCodes.Success, response.status);
        }

        private AdminController GetController()
        {
            return new AdminController(authRepo.Object, adminRepo.Object, opts.Object);
        }

        public static void Init()
        {
            
            authRepo = new Mock<IAuthenticationRepository>();
            adminRepo = new Mock<IAdminRepository>();
            opts = new Mock<IOptionsMonitor<GlobalOptions>>();
            opts.Setup(r => r.CurrentValue).Returns(new GlobalOptions
            {
                AdminAuthExpireHours = 2
            });
            tmpAdmin = new Admin
            {
                Username = "omidd",
                Password = "12345"
            };
        }
    }
}