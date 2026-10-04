using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly HttpClient _client;

    public ReportsController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
    {
        // Default date range: encompass sample data 2024-05-01 to 2024-05-31 or current year
        var start = startDate ?? new DateTime(2024, 5, 1);
        var end = endDate ?? DateTime.Today;

        var model = new ReportVM
        {
            StartDate = start,
            EndDate = end
        };

        if (start > end)
        {
            ModelState.AddModelError("", "Start date must be before or equal to End date.");
            return View(model);
        }

        var url = $"api/Reports/news-statistic?startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}";
        var res = await _client.GetAsync(url);

        if (!res.IsSuccessStatusCode)
        {
            var err = await res.ReadErrorMessageAsync();
            ModelState.AddModelError("", err);
            return View(model);
        }

        var reportData = await res.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

        model.TotalNews = reportData.GetProperty("totalNews").GetInt32();
        model.ActiveCount = reportData.GetProperty("activeCount").GetInt32();
        model.InactiveCount = reportData.GetProperty("inactiveCount").GetInt32();

        if (reportData.TryGetProperty("byCategory", out var byCatProp) && byCatProp.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in byCatProp.EnumerateArray())
            {
                model.ByCategory.Add(new ReportSummaryItem
                {
                    Name = item.GetProperty("categoryName").GetString() ?? "",
                    Count = item.GetProperty("articleCount").GetInt32()
                });
            }
        }

        if (reportData.TryGetProperty("byAuthor", out var byAuthProp) && byAuthProp.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in byAuthProp.EnumerateArray())
            {
                model.ByAuthor.Add(new ReportSummaryItem
                {
                    Name = item.GetProperty("authorName").GetString() ?? "",
                    Count = item.GetProperty("articleCount").GetInt32()
                });
            }
        }

        if (reportData.TryGetProperty("articles", out var articlesProp) && articlesProp.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var a in articlesProp.EnumerateArray())
            {
                var articleVM = new NewsArticleVM
                {
                    NewsArticleId = a.GetProperty("newsArticleId").GetString() ?? "",
                    NewsTitle = a.TryGetProperty("newsTitle", out var t) && t.ValueKind == System.Text.Json.JsonValueKind.String ? t.GetString() ?? "" : "",
                    Headline = a.TryGetProperty("headline", out var h) && h.ValueKind == System.Text.Json.JsonValueKind.String ? h.GetString() ?? "" : "",
                    NewsStatus = a.TryGetProperty("newsStatus", out var s) && s.ValueKind == System.Text.Json.JsonValueKind.True,
                    CreatedDate = a.TryGetProperty("createdDate", out var cd) && cd.TryGetDateTime(out var dt) ? dt : null
                };

                if (a.TryGetProperty("category", out var cat) && cat.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    articleVM.CategoryName = cat.TryGetProperty("categoryName", out var cn) ? cn.GetString() : null;
                }

                if (a.TryGetProperty("createdBy", out var cb) && cb.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    articleVM.CreatedByName = cb.TryGetProperty("accountName", out var an) ? an.GetString() : null;
                }

                model.Articles.Add(articleVM);
            }
        }

        return View(model);
    }
}
