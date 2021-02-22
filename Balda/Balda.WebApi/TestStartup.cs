using System;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

namespace Balda.WebApi
{
    public sealed class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            ConfigureIdentity(services);
            
            services.AddControllers();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<BaldaUserDbContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.TryAddSingleton<ISystemClock, SystemClock>();
            
            var builder = services.AddIdentityCore<BaldaUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<BaldaUserDbContext>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
