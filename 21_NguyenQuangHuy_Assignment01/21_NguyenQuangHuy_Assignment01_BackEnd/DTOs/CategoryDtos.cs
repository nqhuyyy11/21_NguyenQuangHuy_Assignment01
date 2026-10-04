using System.ComponentModel.DataAnnotations;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category description is required.")]
    [StringLength(250, ErrorMessage = "Category description cannot exceed 250 characters.")]
    public string CategoryDesciption { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool? IsActive { get; set; } = true;
}

public class CategoryUpdateDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category description is required.")]
    [StringLength(250, ErrorMessage = "Category description cannot exceed 250 characters.")]
    public string CategoryDesciption { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool? IsActive { get; set; }
}
