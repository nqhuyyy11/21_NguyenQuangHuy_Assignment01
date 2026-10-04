using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;

public class LoginVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

public class AccountVM
{
    [Display(Name = "Account ID")]
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Account Name is required.")]
    [StringLength(100, ErrorMessage = "Account Name cannot exceed 100 characters.")]
    [Display(Name = "Account Name")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    [Display(Name = "Email Address")]
    public string AccountEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, 2, ErrorMessage = "Please select a valid role.")]
    [Display(Name = "Role")]
    public int? AccountRole { get; set; } = 1;

    [Display(Name = "Password")]
    [StringLength(70, MinimumLength = 1, ErrorMessage = "Password must be at least 1 character.")]
    public string? AccountPassword { get; set; }

    public string RoleName => AccountRole switch
    {
        1 => "Staff",
        2 => "Lecturer",
        _ => "Unknown"
    };
}

public class CategoryVM
{
    [Display(Name = "Category ID")]
    public short CategoryId { get; set; }

    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category Description is required.")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    [Display(Name = "Description")]
    public string CategoryDesciption { get; set; } = string.Empty;

    [Display(Name = "Parent Category")]
    public short? ParentCategoryId { get; set; }

    public string? ParentCategoryName { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public CategoryVM? ParentCategory { get; set; }
}

public class TagVM
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public string? Note { get; set; }
}

public class NewsArticleVM
{
    [Display(Name = "Article ID")]
    public string NewsArticleId { get; set; } = string.Empty;

    [Required(ErrorMessage = "News Title is required.")]
    [StringLength(400, ErrorMessage = "Title cannot exceed 400 characters.")]
    [Display(Name = "Title")]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
    [Display(Name = "Headline")]
    public string Headline { get; set; } = string.Empty;

    [Required(ErrorMessage = "News Content is required.")]
    [StringLength(4000, ErrorMessage = "Content cannot exceed 4000 characters.")]
    [Display(Name = "Content")]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "Source cannot exceed 400 characters.")]
    [Display(Name = "News Source")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public short? CategoryId { get; set; }

    public string? CategoryName { get; set; }
    public CategoryVM? Category { get; set; }

    [Display(Name = "Status (Active)")]
    public bool NewsStatus { get; set; } = true;

    [Display(Name = "Created By")]
    public short? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public AccountVM? CreatedBy { get; set; }

    [Display(Name = "Created Date")]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Updated By")]
    public short? UpdatedById { get; set; }

    [Display(Name = "Modified Date")]
    public DateTime? ModifiedDate { get; set; }

    [Display(Name = "Tags")]
    public List<int> TagIds { get; set; } = new();

    public List<TagVM> Tags { get; set; } = new();
}

public class ProfileVM
{
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Account Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    [Display(Name = "Email Address")]
    public string AccountEmail { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [StringLength(70, MinimumLength = 1, ErrorMessage = "New password must be at least 1 character.")]
    [Display(Name = "New Password (leave empty to keep current)")]
    public string? NewPassword { get; set; }
}

public class ReportSummaryItem
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportVM
{
    [Required(ErrorMessage = "Start Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    public int TotalNews { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }

    public List<ReportSummaryItem> ByCategory { get; set; } = new();
    public List<ReportSummaryItem> ByAuthor { get; set; } = new();
    public List<NewsArticleVM> Articles { get; set; } = new();
}
