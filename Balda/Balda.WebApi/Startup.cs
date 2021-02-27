using System;
using Balda.WebApi.Database;
using Balda.WebApi.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
            services.Configure<JwtTokenConfiguration>(Configuration.GetSection(JwtTokenConfiguration.Section));
            services.TryAddSingleton<JwtTokenGenerator>();
            
            var jwt = Configuration.GetSection(JwtTokenConfiguration.Section)
                .Get<JwtTokenConfiguration>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = SecurityKeyGenerator.Generate(jwt.PrivateKey)
            };
            
            services.TryAddSingleton(tokenValidationParameters);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // TODO: require on prod
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenValidationParameters;
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
