using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    }
}