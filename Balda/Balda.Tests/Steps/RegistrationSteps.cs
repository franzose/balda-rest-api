using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private readonly HttpClient _client;
        private readonly UserManager<BaldaUser> _userManager;
        private UserCredentials _credentials;
        private HttpResponseMessage _response;

        public RegistrationSteps(BaldaWebApplicationFactory app)
        {
            _client = app.CreateDefaultClient();
            _userManager = app.GetService<UserManager<BaldaUser>>();
        }
        
        [Given("There are some registered users")]
        public async Task ThereAreRegisteredUsers(Table table)
        {
            foreach (var credentials in table.CreateSet<UserCredentials>())
            {
                await _userManager.CreateAsync(new BaldaUser {UserName = credentials.UserName}, credentials.Password);
            }
        }
        
        [When("I register using the following credentials")]
        public async Task Register(Table table)
        {
            _credentials = table.CreateInstance<UserCredentials>();

            var uri = new Uri("/register", UriKind.Relative);
            var content = new StringContent(JsonSerializer.Serialize(_credentials), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(uri, content);
        }
        
        [Then(@"I should see (\d+) status code")]
        public void AssertResponseStatusCode(int code)
        {
            Assert.Equal((HttpStatusCode) code, _response.StatusCode);
        }

        [Then("I should be registered")]
        public async Task AssertUserIsRegistered()
        {
            await AssertUserAndPassword(_credentials.UserName, _credentials.Password);
        }

        [Then("I should not be registered")]
        public async Task AssertUserIsNotRegistered()
        {
            Assert.Null(await _userManager.FindByNameAsync(_credentials.UserName));
        }

        [Then(@"the ""(.+)"" user should have ""(.+)"" password")]
        public async Task AssertUserAndPassword(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            Assert.IsType<BaldaUser>(user);
            Assert.True(await _userManager.CheckPasswordAsync(user, password));
        }

        [Then(@"I should see the ""(.+)"" message")]
        public async Task AssertResponseMessage(string message)
        {
            var content = await _response.Content.ReadAsStringAsync();
            
            Assert.Equal(message, JsonSerializer.Deserialize<HttpResponse>(content)?.Message);
        }
    }
}