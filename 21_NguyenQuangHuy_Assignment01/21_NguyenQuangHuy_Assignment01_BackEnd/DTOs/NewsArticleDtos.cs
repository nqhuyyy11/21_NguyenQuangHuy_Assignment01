using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;

public class NewsArticleCreateDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(400, ErrorMessage = "Title cannot exceed 400 characters.")]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
    public string Headline { get; set; } = string.Empty;

    [Required(ErrorMessage = "News content is required.")]
    [StringLength(4000, ErrorMessage = "News content cannot exceed 4000 characters.")]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "News source cannot exceed 400 characters.")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public List<int> TagIds { get; set; } = new();
}

public class NewsArticleUpdateDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(400, ErrorMessage = "Title cannot exceed 400 characters.")]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
    public string Headline { get; set; } = string.Empty;

    [Required(ErrorMessage = "News content is required.")]
    [StringLength(4000, ErrorMessage = "News content cannot exceed 4000 characters.")]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "News source cannot exceed 400 characters.")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; }

    public List<int> TagIds { get; set; } = new();
}
