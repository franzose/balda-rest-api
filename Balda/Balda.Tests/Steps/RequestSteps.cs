using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Identity;
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

        [Then(@"I should be authenticated")]
        public void AssertAuthenticated()
            => Assert.Contains(_response.Headers, h =>
                h.Key == "Set-Cookie" && h.Value.FirstOrDefault(v => v.StartsWith("balda_auth")) != null);

        [Then(@"I should not be authenticated")]
        public void AssertNotAuthenticated()
            => Assert.DoesNotContain(_response.Headers, h => h.Key == "Set-Cookie");
    }
}