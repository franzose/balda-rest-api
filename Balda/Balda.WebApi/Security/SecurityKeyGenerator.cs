using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Balda.WebApi.Security
{
    public static class SecurityKeyGenerator
    {
        public static SecurityKey Generate(string privateKey)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(privateKey));
        }
    }
}