using System;
using System.Linq;
using System.Threading.Tasks;
using Balda.WebApi.Controllers.Model;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Balda.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    public sealed class RegistrationController : ControllerBase
    {
        private readonly UserManager<BaldaUser> _userManager;

        public RegistrationController(UserManager<BaldaUser> userManager)
        {
            _userManager = userManager;
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            try
            {
                var user = new BaldaUser {UserName = request.UserName};
                var identity = await _userManager.CreateAsync(user, request.Password);

                return identity.Succeeded
                    ? Ok(new {message = "You have been registered successfully!"})
                    : Problem(identity.Errors.First().Description, "", 401, "Could not register user");
            }
            catch (Exception e)
            {
                // TODO: log error
                return Problem(e.Message, "", 500, "Something went wrong");
            }
        }
    }
}
