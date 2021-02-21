using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Balda.WebApi.Controllers.Model;
using Balda.WebApi.Database;
using Balda.WebApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Balda.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly UserManager<BaldaUser> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<BaldaUser> userManager,
            JwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    return Problem("No user was found for the given credentials", "", 401, "User not found");
                
                var token = await CreateJwtToken(user);
                    
                HttpContext.Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true
                });
                    
                return Ok(new {token});

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                
                return Problem("Your request could not be processed", "", 500, "Something went wrong");
            }
        }

        private async Task<string> CreateJwtToken(BaldaUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var claims = new List<Claim> {new(ClaimsIdentity.DefaultNameClaimType, user.UserName)};
            
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var claimsIdentity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var token = _jwtTokenGenerator.Generate(claimsIdentity);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}