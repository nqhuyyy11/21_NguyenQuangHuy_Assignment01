using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Controllers;

public class AuthController : Controller
{
    private readonly HttpClient _client;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToLocal(returnUrl);
        }
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _client.PostAsJsonAsync("api/Auth/login", new
        {
            email = model.Email,
            password = model.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.ReadErrorMessageAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }

        var loginRes = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var token = loginRes.GetProperty("token").GetString() ?? "";
        var role = loginRes.GetProperty("role").GetString() ?? "";
        var name = loginRes.GetProperty("accountName").GetString() ?? "";
        var email = loginRes.GetProperty("email").GetString() ?? "";
        var accountId = loginRes.GetProperty("accountId").GetInt16().ToString();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, accountId),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("access_token", token)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return role switch
        {
            "Admin" => RedirectToAction("Index", "Accounts"),
            "Staff" => RedirectToAction("Index", "NewsArticles"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpGet, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
