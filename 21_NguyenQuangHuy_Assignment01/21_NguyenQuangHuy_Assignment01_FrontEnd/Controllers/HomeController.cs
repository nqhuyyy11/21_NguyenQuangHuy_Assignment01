using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _client;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index(string? search, short? categoryId)
    {
        // 1. Load active categories for filter
        var categories = new List<CategoryVM>();
        try
        {
            var catRes = await _client.GetFromJsonAsync<ODataResponse<CategoryVM>>("odata/Categories?$filter=IsActive eq true&$orderby=CategoryName");
            if (catRes?.Value != null)
            {
                categories = catRes.Value;
            }
        }
        catch { }
        ViewBag.Categories = categories;
        ViewBag.SelectedCategory = categoryId;
        ViewBag.CurrentSearch = search;

        // 2. Query PublicNews
        var queryFilters = new List<string>();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            queryFilters.Add($"CategoryId eq {categoryId.Value}");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = search.Replace("'", "''");
            queryFilters.Add($"(contains(tolower(NewsTitle), tolower('{escaped}')) or contains(tolower(Headline), tolower('{escaped}')))");
        }

        var url = "odata/PublicNews?$expand=Category,Tags&$orderby=CreatedDate desc";
        if (queryFilters.Count > 0)
        {
            url += "&$filter=" + string.Join(" and ", queryFilters);
        }

        var articles = new List<NewsArticleVM>();
        try
        {
            var res = await _client.GetFromJsonAsync<ODataResponse<NewsArticleVM>>(url);
            if (res?.Value != null)
            {
                articles = res.Value;
            }
        }
        catch { }

        return View(articles);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        try
        {
            var url = $"odata/PublicNews('{Uri.EscapeDataString(id)}')?$expand=Category,CreatedBy,Tags";
            var article = await _client.GetFromJsonAsync<NewsArticleVM>(url);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }
        catch
        {
            return NotFound();
        }
    }
}
