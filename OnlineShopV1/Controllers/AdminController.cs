using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;

namespace OnlineShopV1.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly IOptionsMonitor<GlobalOptions> _options;
        private readonly IAuthenticationRepository _authRepo;
        private readonly IAdminRepository _adminRepo;
        
        public AdminController(
            IAuthenticationRepository authRepo, 
            IAdminRepository adminRepo,
            IOptionsMonitor<GlobalOptions> options)
        {
            _options = options;
            _authRepo = authRepo;
            _adminRepo = adminRepo;
        }

        public async Task<ActionResult<LoginResponse>> Login([FromBody] Admin tmpAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MBadRequest(ModelState));
            }

            var admin = await _adminRepo.GetByUsername(tmpAdmin.Username);

            if (admin == null || !admin.VerifyPassword(tmpAdmin.Password))
            {
                return Unauthorized(new MAuthenticationFailed());
            }

            var auth = await _authRepo.GetByUsername(tmpAdmin.Username);

            if (auth == null)
            {
                // generate new authentication
                auth = new Authentication
                {
                    Username = tmpAdmin.Username,
                    UserType = UserType.Admin
                };

                auth.GenerateNewCode();
                auth.SetExpiresFromNow(_options.CurrentValue.AdminAuthExpireHours);
                await _authRepo.AddAuthentication(auth);
            }
            else if (auth.IsExpired())
            {
                auth.GenerateNewCode();
                auth.SetExpiresFromNow(_options.CurrentValue.AdminAuthExpireHours);
                await _authRepo.UpdateAuthentication(auth);
            }

            return Ok(new LoginResponse(auth.Code));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<Response>> Logout()
        {

            var auth = HttpContext.Items["auth"] as Authentication;
            
            await _authRepo.Remove(auth);

            return Ok(new DefaultResponse());
        }

        [HttpPost("change-pass")]
        public async Task<ActionResult<Response>> ChangePassword([FromBody] ChangePasswordRequest chPassReq)
        {

            
            if (!ModelState.IsValid)
            {
                return BadRequest(new MBadRequest(ModelState));
            }

            var admin = HttpContext.Items["admin"] as Admin;

            if (!admin.VerifyPassword(chPassReq.OldPassword))
            {
                return BadRequest(new DefaultResponse(StatusCodes.MissMatchPassword));
            }

            admin.Password = chPassReq.NewPassword;
            admin.HashPassword();

            await _adminRepo.Update(admin);
            
            return Ok(new DefaultResponse());
        }
    }


    public class ChangePasswordRequest
    {
        [MinLength(5)]
        [Required]
        public String OldPassword { get; set; }
        
        [Required]
        [MinLength(5)]
        public String NewPassword { get; set; }
    }
}