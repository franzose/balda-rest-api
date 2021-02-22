using Balda.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Balda.Tests
{
    public sealed class BaldaWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        public T GetService<T>() where T : class
        {
            return (T) Services.GetService(typeof(T));
        }
        
        protected override IHostBuilder CreateHostBuilder()
            => Host.CreateDefaultBuilder(System.Array.Empty<string>())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<TestStartup>(); })
                .UseEnvironment("Test");
    }
}