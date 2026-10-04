using System.Security.Claims;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

[Authorize(Roles = "Staff,Lecturer")]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IAccountRepository _accountRepo;

    public ProfileController(IAccountRepository accountRepo)
    {
        _accountRepo = accountRepo;
    }

    [HttpGet]
    public IActionResult GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var account = _accountRepo.GetById(userId);
        if (account == null)
        {
            return NotFound(new { message = "User account not found." });
        }

        return Ok(new
        {
            account.AccountId,
            account.AccountName,
            account.AccountEmail,
            Role = account.AccountRole == 1 ? "Staff" : "Lecturer"
        });
    }

    [HttpPut]
    public IActionResult UpdateProfile([FromBody] ProfileUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var account = _accountRepo.GetById(userId);
        if (account == null)
        {
            return NotFound(new { message = "User account not found." });
        }

        // Email uniqueness check
        var existingWithEmail = _accountRepo.GetByEmail(dto.AccountEmail);
        if (existingWithEmail != null && existingWithEmail.AccountId != userId)
        {
            return BadRequest(new { message = "Email is already taken by another account." });
        }

        account.AccountName = dto.AccountName.Trim();
        account.AccountEmail = dto.AccountEmail.Trim();

        // Optional password change
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (!string.IsNullOrEmpty(dto.CurrentPassword) && account.AccountPassword != dto.CurrentPassword)
            {
                return BadRequest(new { message = "Current password is incorrect." });
            }
            account.AccountPassword = dto.NewPassword;
        }

        _accountRepo.Update(account);
        return Ok(new { message = "Profile updated successfully." });
    }
}
