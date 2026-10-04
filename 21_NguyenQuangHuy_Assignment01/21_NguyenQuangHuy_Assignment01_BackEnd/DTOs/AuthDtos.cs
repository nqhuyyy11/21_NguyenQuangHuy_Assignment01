using System;
using System.ComponentModel.DataAnnotations;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
