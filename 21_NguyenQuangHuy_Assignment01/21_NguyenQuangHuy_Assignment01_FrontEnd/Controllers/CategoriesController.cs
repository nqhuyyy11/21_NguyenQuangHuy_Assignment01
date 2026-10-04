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
public class CategoriesController : Controller
{
    private readonly HttpClient _client;

    public CategoriesController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.CurrentSearch = search;
        var url = "odata/Categories?$expand=ParentCategory&$orderby=CategoryId";

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = search.Replace("'", "''");
            url += $"&$filter=contains(tolower(CategoryName), tolower('{escaped}'))";
        }

        var categories = new List<CategoryVM>();
        try
        {
            var res = await _client.GetFromJsonAsync<ODataResponse<CategoryVM>>(url);
            if (res?.Value != null)
            {
                categories = res.Value;
            }
        }
        catch { }

        return View(categories);
    }

    private async Task LoadParents(short? currentId = null)
    {
        var list = new List<CategoryVM>();
        try
        {
            var res = await _client.GetFromJsonAsync<ODataResponse<CategoryVM>>("odata/Categories?$orderby=CategoryName");
            if (res?.Value != null)
            {
                list = res.Value;
            }
        }
        catch { }
        ViewBag.Parents = list;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadParents();
        return PartialView("_Form", new CategoryVM { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            await LoadParents();
            return PartialView("_Form", model);
        }

        var res = await _client.PostAsJsonAsync("api/Categories", new
        {
            categoryName = model.CategoryName,
            categoryDesciption = model.CategoryDesciption,
            parentCategoryId = model.ParentCategoryId,
            isActive = model.IsActive
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Category created successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        await LoadParents();
        return PartialView("_Form", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(short id)
    {
        try
        {
            var cat = await _client.GetFromJsonAsync<CategoryVM>($"odata/Categories({id})");
            if (cat == null)
            {
                return NotFound();
            }
            await LoadParents(id);
            return PartialView("_Form", cat);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short id, CategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            await LoadParents(id);
            return PartialView("_Form", model);
        }

        var res = await _client.PutAsJsonAsync($"api/Categories/{id}", new
        {
            categoryName = model.CategoryName,
            categoryDesciption = model.CategoryDesciption,
            parentCategoryId = model.ParentCategoryId,
            isActive = model.IsActive
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Category updated successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        await LoadParents(id);
        return PartialView("_Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(short id)
    {
        var res = await _client.DeleteAsync($"api/Categories/{id}");
        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Category deleted successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        return Json(new { success = false, message = err });
    }
}
