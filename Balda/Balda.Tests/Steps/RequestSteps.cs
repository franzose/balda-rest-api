using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Balda.Tests.Support;
using Microsoft.IdentityModel.Tokens;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Abstractions;

namespace Balda.Tests.Steps
{
    [Binding]
    public sealed class RequestSteps : IClassFixture<BaldaWebApplicationFactory>
    {
        private readonly ITestOutputHelper _output;
        private readonly BaldaWebApplicationFactory _app;
        private readonly HttpClient _client;
        private HttpRequestMessage _request;
        private HttpResponseMessage _response;
        
        public RequestSteps(ITestOutputHelper output, BaldaWebApplicationFactory app)
        {
            _output = output;
            _app = app;
            _client = app.CreateDefaultClient();
        }

        [When(@"I send (GET|POST|PUT|PATCH|DELETE) request to ""(.+)"" with")]
        public async Task SendRequest(string method, string endpoint, string json)
        {
            _request = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            
            _response = await _client.SendAsync(_request);
        }
        
        [Then(@"I should see (\d+) status code")]
        public void AssertResponseStatusCode(int code)
            => Assert.Equal((HttpStatusCode) code, _response.StatusCode);

        [Then(@"I should see the ""(.+)"" message")]
        public async Task AssertResponseMessage(string message)
        {
            var content = await _response.Content.ReadAsStringAsync();

            Assert.Contains(message, content);
        }

        [Then(@"I should be authenticated as the ""(.+)"" user")]
        public async Task AssertAuthenticatedAs(string username)
        {
            var token = await GetJwtTokenFromResponse();

            AssertUserIsAuthenticated(username, token);
            AssertHeadersContainTokenCookie(token);
        }

        [Then(@"I should not be authenticated")]
        public async Task AssertNotAuthenticatedAs()
        {
            Assert.Empty(await GetJwtTokenFromResponse());
            AssertHeadersDoNotContainTokenCookie();
        }

        private async Task<string> GetJwtTokenFromResponse()
        {
            var content = await _response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<HttpResponse>(content)?.Token ?? "";
        }

        private void AssertUserIsAuthenticated(string username, string token)
        {
            var validationParams = _app.GetService<TokenValidationParameters>();
            var claims = new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParams, out var securityToken)
                .Claims;
            
            foreach (var claim in claims)
            {
                Assert.Equal(username, claim.Subject?.Name ?? "");
                Assert.True(claim.Subject?.IsAuthenticated);
            }
        }

        private void AssertHeadersContainTokenCookie(string token)
        {
            Assert.Contains(_response.Headers, h =>
                h.Key == "Set-Cookie" && h.Value.First() == $"token={token}; path=/; httponly");
        }

        private void AssertHeadersDoNotContainTokenCookie()
        {
            Assert.DoesNotContain(_response.Headers, h => h.Key == "Set-Cookie");
        }
    }
}