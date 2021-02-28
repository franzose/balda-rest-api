using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Balda.WebApi.Controllers.Model;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Balda.WebApi.Controllers
{
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly UserManager<BaldaUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<BaldaUser> userManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    return Problem("No user was found for the given credentials", "", 401, "User not found");

                var principal = await CreateClaimsPrincipal(user);
                var props = new AuthenticationProperties { AllowRefresh = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);

                return Problem("Your request could not be processed", "", 500, "Something went wrong");
            }
        }

        private async Task<ClaimsPrincipal> CreateClaimsPrincipal(BaldaUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>{new (ClaimTypes.Name, user.UserName)};

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        }
    }
}