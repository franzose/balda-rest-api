using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace Balda.WebApi.Security
{
    public sealed class JwtTokenGenerator
    {
        private readonly JwtTokenConfiguration _configuration;

        public JwtTokenGenerator(IOptions<JwtTokenConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public JwtSecurityToken Generate(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(_configuration.Lifetime));
            var credentials = new SigningCredentials(
                SecurityKeyGenerator.Generate(_configuration.PrivateKey),
                SecurityAlgorithms.HmacSha512);
            
            return new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: expires,
                signingCredentials: credentials);
        }
    }

    public sealed class JwtTokenConfiguration
    {
        public const string Section = "JWT";

        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public double Lifetime { get; set; } = 15;
        public string PrivateKey { get; set; } = "";
    }
}