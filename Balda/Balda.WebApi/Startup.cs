using System;
using Balda.WebApi.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Balda.WebApi
{
    public sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            ConfigureIdentity(services);
            ConfigureAuthentication(services);
            
            services.AddControllers();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<BaldaUserDbContext>(
                opt => opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
            );
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

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                    options.AccessDeniedPath = "/forbidden";
                    options.Cookie.Name = "balda_auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsProduction())
            {
                app.UseHttpsRedirection();
            }
            
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
