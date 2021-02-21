using Microsoft.AspNetCore.Mvc;

namespace Balda.WebApi.Controllers.Model
{
    public sealed class RegistrationRequest
    {
        [BindProperty(Name = "username")] public string UserName { get; set; } = "";
        [BindProperty(Name = "password")] public string Password { get; set; } = "";
    }
}