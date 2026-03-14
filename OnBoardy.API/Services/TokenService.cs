using Microsoft.IdentityModel.Tokens;
using OnBoardy.API.Data;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnBoardy.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public TokenService(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        public string CreateAccessToken(User user)
        {
            var secretKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT Secret Key is missing from configuration!");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (!double.TryParse(_config["Jwt:ExpireMinutes"], out double expireMinutes))
            {
                expireMinutes = 60;
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string ipAddress)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            if (!double.TryParse(_config["Jwt:RefreshTokenExpireDays"], out double refreshTokenExpireDays))
            {
                refreshTokenExpireDays = 60;
            }

            var refresh = new RefreshToken
            {
                Token = token,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpireDays), //Should be moved to Configuration? (idk)
                IpAddress = ipAddress
            };

            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return refresh;
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}