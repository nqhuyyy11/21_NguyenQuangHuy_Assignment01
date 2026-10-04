using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessObjects.Models;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;

public class ReportRequest : IValidatableObject
{
    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            yield return new ValidationResult(
                "Start date must be before or equal to end date.",
                new[] { nameof(StartDate), nameof(EndDate) }
            );
        }
    }
}

public class ReportCategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ArticleCount { get; set; }
}

public class ReportAuthorSummaryDto
{
    public string AuthorName { get; set; } = string.Empty;
    public int ArticleCount { get; set; }
}

public class NewsReportDto
{
    public int TotalNews { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public List<ReportCategorySummaryDto> ByCategory { get; set; } = new();
    public List<ReportAuthorSummaryDto> ByAuthor { get; set; } = new();
    public List<NewsArticle> Articles { get; set; } = new();
}
