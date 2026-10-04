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
public class AccountsController : Controller
{
    private readonly HttpClient _client;

    public AccountsController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.CurrentSearch = search;
        var url = "odata/SystemAccounts?$orderby=AccountId";

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = search.Replace("'", "''");
            url += $"&$filter=contains(tolower(AccountName), tolower('{escaped}')) or contains(tolower(AccountEmail), tolower('{escaped}'))";
        }

        var accounts = new List<AccountVM>();
        try
        {
            var res = await _client.GetFromJsonAsync<ODataResponse<AccountVM>>(url);
            if (res?.Value != null)
            {
                accounts = res.Value;
            }
        }
        catch { }

        return View(accounts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_Form", new AccountVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountVM model)
    {
        if (string.IsNullOrWhiteSpace(model.AccountPassword))
        {
            ModelState.AddModelError("AccountPassword", "Password is required for new accounts.");
        }

        if (!ModelState.IsValid)
        {
            return PartialView("_Form", model);
        }

        var res = await _client.PostAsJsonAsync("api/SystemAccounts", new
        {
            accountName = model.AccountName,
            accountEmail = model.AccountEmail,
            accountRole = model.AccountRole,
            accountPassword = model.AccountPassword
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Account created successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        return PartialView("_Form", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(short id)
    {
        try
        {
            var account = await _client.GetFromJsonAsync<AccountVM>($"odata/SystemAccounts({id})");
            if (account == null)
            {
                return NotFound();
            }
            return PartialView("_Form", account);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short id, AccountVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_Form", model);
        }

        var res = await _client.PutAsJsonAsync($"api/SystemAccounts/{id}", new
        {
            accountName = model.AccountName,
            accountEmail = model.AccountEmail,
            accountRole = model.AccountRole,
            accountPassword = model.AccountPassword
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Account updated successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        return PartialView("_Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(short id)
    {
        var res = await _client.DeleteAsync($"api/SystemAccounts/{id}");
        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Account deleted successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        return Json(new { success = false, message = err });
    }
}
