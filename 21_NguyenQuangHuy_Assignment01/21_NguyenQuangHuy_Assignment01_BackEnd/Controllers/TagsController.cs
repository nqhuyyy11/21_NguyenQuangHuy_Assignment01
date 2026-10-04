using System.Linq;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

[Authorize(Roles = "Staff,Admin")]
public class TagsController : ODataController
{
    private readonly ITagRepository _tagRepo;

    public TagsController(ITagRepository tagRepo)
    {
        _tagRepo = tagRepo;
    }

    [EnableQuery]
    [HttpGet("odata/Tags")]
    public IActionResult Get()
    {
        return Ok(_tagRepo.GetAll().AsQueryable());
    }

    [EnableQuery]
    [HttpGet("odata/Tags({key})")]
    public IActionResult Get([FromRoute] int key)
    {
        var tag = _tagRepo.GetById(key);
        if (tag == null)
        {
            return NotFound(new { message = $"Tag with ID {key} not found." });
        }
        return Ok(tag);
    }

    [HttpPost("api/Tags")]
    public IActionResult Create([FromBody] TagCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tag = new Tag
        {
            TagName = dto.TagName.Trim(),
            Note = dto.Note?.Trim()
        };

        _tagRepo.Add(tag);
        return Ok(tag);
    }
}
