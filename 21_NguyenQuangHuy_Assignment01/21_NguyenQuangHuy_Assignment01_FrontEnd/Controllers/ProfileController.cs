using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;
using _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = "Staff,Lecturer")]
public class ProfileController : Controller
{
    private readonly HttpClient _client;

    public ProfileController(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Api");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var profile = await _client.GetFromJsonAsync<ProfileVM>("api/Profile");
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        try
        {
            var profile = await _client.GetFromJsonAsync<ProfileVM>("api/Profile");
            if (profile == null)
            {
                return NotFound();
            }
            return PartialView("_EditModal", profile);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditModal", model);
        }

        var res = await _client.PutAsJsonAsync("api/Profile", new
        {
            accountName = model.AccountName,
            accountEmail = model.AccountEmail,
            currentPassword = model.CurrentPassword,
            newPassword = model.NewPassword
        });

        if (res.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Profile updated successfully." });
        }

        var err = await res.ReadErrorMessageAsync();
        ModelState.AddModelError("", err);
        return PartialView("_EditModal", model);
    }
}
