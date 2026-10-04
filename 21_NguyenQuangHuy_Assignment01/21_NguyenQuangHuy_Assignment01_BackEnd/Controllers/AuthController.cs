using System;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using _21_NguyenQuangHuy_Assignment01_BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IAccountRepository _accountRepo;
    private readonly JwtTokenService _jwtService;

    public AuthController(IConfiguration config, IAccountRepository accountRepo, JwtTokenService jwtService)
    {
        _config = config;
        _accountRepo = accountRepo;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 1. Check Admin Account from appsettings.json
        var adminConfig = _config.GetSection("AdminAccount");
        var adminEmail = adminConfig["Email"];
        var adminPassword = adminConfig["Password"];

        if (!string.IsNullOrEmpty(adminEmail) &&
            dto.Email.Trim().Equals(adminEmail.Trim(), StringComparison.OrdinalIgnoreCase) &&
            dto.Password == adminPassword)
        {
            var (token, expiresAt) = _jwtService.CreateToken(0, "Administrator", adminEmail, "Admin");
            return Ok(new LoginResponseDto
            {
                Token = token,
                AccountId = 0,
                AccountName = "Administrator",
                Email = adminEmail,
                Role = "Admin",
                ExpiresAt = expiresAt
            });
        }

        // 2. Check Database SystemAccount
        var user = _accountRepo.Login(dto.Email, dto.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roleName = user.AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "Unknown"
        };

        var (userToken, userExpiresAt) = _jwtService.CreateToken(user.AccountId, user.AccountName, user.AccountEmail, roleName);
        return Ok(new LoginResponseDto
        {
            Token = userToken,
            AccountId = user.AccountId,
            AccountName = user.AccountName,
            Email = user.AccountEmail,
            Role = roleName,
            ExpiresAt = userExpiresAt
        });
    }
}
