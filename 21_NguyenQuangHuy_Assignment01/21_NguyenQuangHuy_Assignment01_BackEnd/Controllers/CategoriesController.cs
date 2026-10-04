using System.Linq;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

public class CategoriesController : ODataController
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoriesController(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    [EnableQuery]
    [AllowAnonymous]
    [HttpGet("odata/Categories")]
    public IActionResult Get()
    {
        return Ok(_categoryRepo.GetAll().AsQueryable());
    }

    [EnableQuery]
    [AllowAnonymous]
    [HttpGet("odata/Categories({key})")]
    public IActionResult Get([FromRoute] short key)
    {
        var category = _categoryRepo.GetById(key);
        if (category == null)
        {
            return NotFound(new { message = $"Category with ID {key} not found." });
        }
        return Ok(category);
    }

    [Authorize(Roles = "Staff")]
    [HttpPost("api/Categories")]
    public IActionResult Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId.Value > 0)
        {
            var parent = _categoryRepo.GetById(dto.ParentCategoryId.Value);
            if (parent == null)
            {
                return BadRequest(new { message = "Selected parent category does not exist." });
            }
        }

        var category = new Category
        {
            CategoryName = dto.CategoryName.Trim(),
            CategoryDesciption = dto.CategoryDesciption.Trim(),
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = dto.IsActive ?? true
        };

        _categoryRepo.Add(category);
        return Ok(category);
    }

    [Authorize(Roles = "Staff")]
    [HttpPut("api/Categories/{id}")]
    public IActionResult Update(short id, [FromBody] CategoryUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = _categoryRepo.GetById(id);
        if (category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId.Value > 0 && dto.ParentCategoryId.Value != id)
        {
            var parent = _categoryRepo.GetById(dto.ParentCategoryId.Value);
            if (parent == null)
            {
                return BadRequest(new { message = "Selected parent category does not exist." });
            }
        }

        category.CategoryName = dto.CategoryName.Trim();
        category.CategoryDesciption = dto.CategoryDesciption.Trim();
        category.ParentCategoryId = dto.ParentCategoryId;
        category.IsActive = dto.IsActive;

        _categoryRepo.Update(category);
        return NoContent();
    }

    [Authorize(Roles = "Staff")]
    [HttpDelete("api/Categories/{id}")]
    public IActionResult Delete(short id)
    {
        var category = _categoryRepo.GetById(id);
        if (category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        if (!_categoryRepo.CanDelete(id))
        {
            return BadRequest(new { message = "Cannot delete: category is used by news articles or has sub-categories." });
        }

        _categoryRepo.Delete(id);
        return NoContent();
    }
}
