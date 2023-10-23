using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Soup_Backend.Logic
{
    public class AuthenticationLogic
    {
        private IConfiguration _configuration;
        public AuthenticationLogic(IConfiguration configuration) {
            _configuration = configuration;
        }
        public string GenerateJWTBearer(String user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtConfig:Key").Value)
                );

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user)
            };

            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = signingCredential
            };
            return "";
        }
    }
}
