using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assessment.Infrastructure.Security
{
    public class CandidateTokenGenerator
    {
        private readonly IConfiguration _config;

        public CandidateTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        // Token lives for the test duration + 30 min buffer
        public (string Token, DateTime ExpiresAtUtc) Generate(Guid testId, Guid applicantId, int durationMinutes)
        {
            var jwtKey = _config["Jwt:Key"]!;
            var jwtIssuer = _config["Jwt:Issuer"]!;
            var jwtAudience = _config["Jwt:Audience"]!;

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(durationMinutes + 30);

            var claims = new[]
            {
                new Claim("testId",      testId.ToString()),
                new Claim("applicantId", applicantId.ToString()),
                new Claim(ClaimTypes.Role, "Candidate"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }
    }
}