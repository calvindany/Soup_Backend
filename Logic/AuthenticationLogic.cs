using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Soup_Backend.DTOs.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
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
        public string GenerateJWTBearer(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtConfig:Key").Value)
                );

            var claims = new Claim[]
            {
                new Claim("user_id", userId.ToString())
            };

            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = signingCredential
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}
