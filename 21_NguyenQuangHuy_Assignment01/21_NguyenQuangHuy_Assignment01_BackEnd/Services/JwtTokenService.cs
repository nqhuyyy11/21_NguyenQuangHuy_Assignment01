using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Services;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expiresAt) CreateToken(short accountId, string? accountName, string? email, string role)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var secretKey = jwtSettings["Key"] ?? "ThisIsASuperSecretKeyForFUNews_PRN232_AtLeast32Chars!";
        var issuer = jwtSettings["Issuer"] ?? "FUNewsManagement.API";
        var audience = jwtSettings["Audience"] ?? "FUNewsManagement.Client";
        var expireMinutes = int.TryParse(jwtSettings["ExpireMinutes"], out var exp) ? exp : 120;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
            new Claim(ClaimTypes.Name, accountName ?? ""),
            new Claim(ClaimTypes.Email, email ?? ""),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
