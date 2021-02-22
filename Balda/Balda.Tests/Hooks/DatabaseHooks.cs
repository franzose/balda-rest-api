using System.Threading.Tasks;
using Balda.WebApi.Database;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Xunit;

namespace Balda.Tests.Hooks
{
    [Binding]
    public sealed class DatabaseHooks : IClassFixture<BaldaWebApplicationFactory>
    {
        private readonly BaldaWebApplicationFactory _app;

        public DatabaseHooks(BaldaWebApplicationFactory app) => _app = app;

        [BeforeScenario]
        public async Task Migrate()
        {
            var context = _app.GetService<BaldaUserDbContext>();

            await context.Database.ExecuteSqlRawAsync("DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
            await context.Database.MigrateAsync();
        }
    }
}