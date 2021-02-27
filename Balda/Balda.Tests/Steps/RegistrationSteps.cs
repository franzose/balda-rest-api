using System.Threading.Tasks;
using Balda.Tests.Support;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Identity;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Balda.Tests.Steps
{
    [Binding]
    public sealed class RegistrationSteps : IClassFixture<BaldaWebApplicationFactory>
    {
        private readonly UserManager<BaldaUser> _userManager;

        public RegistrationSteps(BaldaWebApplicationFactory app)
            => _userManager = app.GetService<UserManager<BaldaUser>>();

        [Given("There are some registered users")]
        public async Task ThereAreRegisteredUsers(Table table)
        {
            foreach (var credentials in table.CreateSet<UserCredentials>())
            {
                await _userManager.CreateAsync(new BaldaUser {UserName = credentials.UserName}, credentials.Password);
            }
        }

        [Then(@"I should be registered as ""(.+)"" with the ""(.+)"" password")]
        public async Task AssertUserIsRegistered(string username, string password)
        {
            await AssertUserAndPassword(username, password);
        }

        [Then(@"I should not be registered as ""(.*)""")]
        public async Task AssertUserIsNotRegistered(string username)
        {
            Assert.Null(await _userManager.FindByNameAsync(username));
        }

        [Then(@"the ""(.+)"" user should have ""(.+)"" password")]
        public async Task AssertUserAndPassword(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            Assert.IsType<BaldaUser>(user);
            Assert.True(await _userManager.CheckPasswordAsync(user, password));
        }
    }
}