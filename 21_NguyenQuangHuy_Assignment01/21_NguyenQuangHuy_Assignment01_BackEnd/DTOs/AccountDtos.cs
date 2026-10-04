using System.ComponentModel.DataAnnotations;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;

public class AccountCreateDto
{
    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    public string AccountEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer).")]
    public int? AccountRole { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(70, MinimumLength = 1, ErrorMessage = "Password must be at least 1 character.")]
    public string AccountPassword { get; set; } = string.Empty;
}

public class AccountUpdateDto
{
    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    public string AccountEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer).")]
    public int? AccountRole { get; set; }

    [StringLength(70, MinimumLength = 1, ErrorMessage = "Password must be at least 1 character if provided.")]
    public string? AccountPassword { get; set; }
}

public class ProfileUpdateDto
{
    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    public string AccountEmail { get; set; } = string.Empty;

    public string? CurrentPassword { get; set; }

    [StringLength(70, MinimumLength = 1, ErrorMessage = "New password must be at least 1 character.")]
    public string? NewPassword { get; set; }
}
