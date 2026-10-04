using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = "Staff")]
public class NewsArticlesController : Controller
{
    private readonly HttpClient _client;

    public NewsArticlesController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.CurrentSearch = search;
        var url = "odata/NewsArticles?$expand=Category,CreatedBy,Tags&$orderby=CreatedDate desc";

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = search.Replace("'", "''");
            url += $"&$filter=contains(tolower(NewsTitle), tolower('{escaped}')) or contains(tolower(Headline), tolower('{escaped}'))";
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

    public async Task<IActionResult> History()
    {
        var articles = new List<NewsArticleVM>();
        try
        {
            var res = await _client.GetFromJsonAsync<List<NewsArticleVM>>("api/NewsArticles/my-history");
            if (res != null)
            {
                articles = res;
            }
        }
        catch { }

        return View(articles);
    }

    private async Task LoadCategoriesAndTags()
    {
        // 1. Only active categories (Rule S2, BR-N4)
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

        // 2. All tags
        var tags = new List<TagVM>();
        try
        {
            var tagRes = await _client.GetFromJsonAsync<ODataResponse<TagVM>>("odata/Tags?$orderby=TagName");
            if (tagRes?.Value != null)
            {
                tags = tagRes.Value;
            }
        }
        catch { }
        ViewBag.Tags = tags;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAndTags();
        return PartialView("_Form", new NewsArticleVM { NewsStatus = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsArticleVM model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndTags();
            return PartialView("_Form", model);
        }

        var res = await _client.PostAsJsonAsync("api/NewsArticles", new
        {
            newsTitle = model.NewsTitle,
            headline = model.Headline,
            newsContent = model.NewsContent,
            newsSource = model.NewsSource,
            categoryId = model.CategoryId,
            newsStatus = model.NewsStatus,
            tagIds = model.TagIds
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "News article created successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        await LoadCategoriesAndTags();
        return PartialView("_Form", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var url = $"odata/NewsArticles('{Uri.EscapeDataString(id)}')?$expand=Category,Tags";
            var news = await _client.GetFromJsonAsync<NewsArticleVM>(url);
            if (news == null)
            {
                return NotFound();
            }

            if (news.Tags != null)
            {
                news.TagIds = news.Tags.Select(t => t.TagId).ToList();
            }

            await LoadCategoriesAndTags();
            return PartialView("_Form", news);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, NewsArticleVM model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndTags();
            return PartialView("_Form", model);
        }

        var res = await _client.PutAsJsonAsync($"api/NewsArticles/{Uri.EscapeDataString(id)}", new
        {
            newsTitle = model.NewsTitle,
            headline = model.Headline,
            newsContent = model.NewsContent,
            newsSource = model.NewsSource,
            categoryId = model.CategoryId,
            newsStatus = model.NewsStatus,
            tagIds = model.TagIds
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "News article updated successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        await LoadCategoriesAndTags();
        return PartialView("_Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var res = await _client.DeleteAsync($"api/NewsArticles/{Uri.EscapeDataString(id)}");
        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "News article deleted successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        return Json(new { success = false, message = err });
    }
}
