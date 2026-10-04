using System;
using System.Linq;
using System.Security.Claims;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

public class NewsArticlesController : ODataController
{
    private readonly INewsArticleRepository _newsRepo;
    private readonly ICategoryRepository _categoryRepo;

    public NewsArticlesController(INewsArticleRepository newsRepo, ICategoryRepository categoryRepo)
    {
        _newsRepo = newsRepo;
        _categoryRepo = categoryRepo;
    }

    [EnableQuery]
    [AllowAnonymous]
    [HttpGet("odata/PublicNews")]
    public IActionResult GetPublicNews()
    {
        return Ok(_newsRepo.GetActive().AsQueryable());
    }

    [EnableQuery]
    [AllowAnonymous]
    [HttpGet("odata/PublicNews({key})")]
    public IActionResult GetPublicNewsById([FromODataUri] string key)
    {
        var news = _newsRepo.GetById(key);
        if (news == null || news.NewsStatus != true)
        {
            return NotFound(new { message = $"Active article with ID '{key}' not found." });
        }
        return Ok(news);
    }

    [EnableQuery]
    [Authorize(Roles = "Staff,Admin")]
    [HttpGet("odata/NewsArticles")]
    public IActionResult Get()
    {
        return Ok(_newsRepo.GetAll().AsQueryable());
    }

    [EnableQuery]
    [Authorize(Roles = "Staff,Admin")]
    [HttpGet("odata/NewsArticles('{key}')")]
    public IActionResult Get([FromRoute] string key)
    {
        var news = _newsRepo.GetById(key);
        if (news == null)
        {
            return NotFound(new { message = $"Article with ID '{key}' not found." });
        }
        return Ok(news);
    }

    [Authorize(Roles = "Staff")]
    [HttpGet("api/NewsArticles/my-history")]
    public IActionResult GetMyHistory()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var history = _newsRepo.GetByCreator(userId);
        return Ok(history);
    }

    [Authorize(Roles = "Staff")]
    [HttpPost("api/NewsArticles")]
    public IActionResult Create([FromBody] NewsArticleCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        if (dto.CategoryId.HasValue)
        {
            var category = _categoryRepo.GetById(dto.CategoryId.Value);
            if (category == null || category.IsActive != true)
            {
                return BadRequest(new { message = "Selected category does not exist or is inactive." });
            }
        }

        var news = new NewsArticle
        {
            NewsTitle = dto.NewsTitle.Trim(),
            Headline = dto.Headline.Trim(),
            NewsContent = dto.NewsContent.Trim(),
            NewsSource = dto.NewsSource?.Trim(),
            CategoryId = dto.CategoryId,
            NewsStatus = dto.NewsStatus,
            CreatedById = userId,
            CreatedDate = DateTime.Now,
            UpdatedById = null,
            ModifiedDate = null
        };

        _newsRepo.Add(news, dto.TagIds);
        return Ok(news);
    }

    [Authorize(Roles = "Staff")]
    [HttpPut("api/NewsArticles/{id}")]
    public IActionResult Update(string id, [FromBody] NewsArticleUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var existing = _newsRepo.GetById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Article with ID '{id}' not found." });
        }

        if (dto.CategoryId.HasValue)
        {
            var category = _categoryRepo.GetById(dto.CategoryId.Value);
            if (category == null || category.IsActive != true)
            {
                return BadRequest(new { message = "Selected category does not exist or is inactive." });
            }
        }

        existing.NewsTitle = dto.NewsTitle.Trim();
        existing.Headline = dto.Headline.Trim();
        existing.NewsContent = dto.NewsContent.Trim();
        existing.NewsSource = dto.NewsSource?.Trim();
        existing.CategoryId = dto.CategoryId;
        existing.NewsStatus = dto.NewsStatus;
        existing.UpdatedById = userId;
        existing.ModifiedDate = DateTime.Now;

        _newsRepo.Update(existing, dto.TagIds);
        return NoContent();
    }

    [Authorize(Roles = "Staff")]
    [HttpDelete("api/NewsArticles/{id}")]
    public IActionResult Delete(string id)
    {
        var existing = _newsRepo.GetById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Article with ID '{id}' not found." });
        }

        _newsRepo.Delete(id);
        return NoContent();
    }
}
