using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using core.Data.Models;

namespace core.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJsonWebToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        if (!int.TryParse(_configuration["Security:Jwt:ExpirationMinutes"], out var expirationMinutes))
            throw new ArgumentException("Failed to parse JWT Expiration Minutes [Security:Jwt:ExpirationMinutes]");

        var token = new JwtSecurityToken(_configuration["Security:Jwt:Issuer"],
            _configuration["Security:Jwt:Audience"],
            new List<Claim>
            {
                new(ClaimTypes.Email, user.Email)
            },
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface ITokenService
{
    public string GenerateJsonWebToken(User user);
}