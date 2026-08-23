using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.Categories;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var category = await _categoryService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }
}
