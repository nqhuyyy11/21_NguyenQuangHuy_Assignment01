using System.Linq;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly INewsArticleRepository _newsRepo;

    public ReportsController(INewsArticleRepository newsRepo)
    {
        _newsRepo = newsRepo;
    }

    [HttpGet("news-statistic")]
    public IActionResult GetNewsStatistic([FromQuery] ReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var articles = _newsRepo.GetByDateRange(request.StartDate!.Value, request.EndDate!.Value);

        var byCategory = articles
            .GroupBy(a => a.Category?.CategoryName ?? "Unknown")
            .Select(g => new ReportCategorySummaryDto
            {
                CategoryName = g.Key,
                ArticleCount = g.Count()
            })
            .OrderByDescending(c => c.ArticleCount)
            .ToList();

        var byAuthor = articles
            .GroupBy(a => a.CreatedBy?.AccountName ?? "Unknown")
            .Select(g => new ReportAuthorSummaryDto
            {
                AuthorName = g.Key,
                ArticleCount = g.Count()
            })
            .OrderByDescending(a => a.ArticleCount)
            .ToList();

        var report = new NewsReportDto
        {
            TotalNews = articles.Count,
            ActiveCount = articles.Count(a => a.NewsStatus == true),
            InactiveCount = articles.Count(a => a.NewsStatus != true),
            ByCategory = byCategory,
            ByAuthor = byAuthor,
            Articles = articles // already sorted descending by CreatedDate in DAO
        };

        return Ok(report);
    }
}
