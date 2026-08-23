using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.Categories;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public CategoryService(ApplicationDbContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryRequest dto, string userId)
    {
        if (dto.ParentCategoryId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == dto.ParentCategoryId.Value);

                if (!parentExists)
                    throw new NotFoundException($"Parent Category with ID {dto.ParentCategoryId.Value} not found.");
            }

        var duplicateName = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.ParentCategoryId == dto.ParentCategoryId);

        if (duplicateName)
            throw new ConflictException($"A category named '{dto.Name}' already exists under the same parent.");

        var category = new Category
        {
            Name = dto.Name,
            ParentCategoryId = dto.ParentCategoryId
        };

        _context.Categories.Add(category);
        await _activityLogService.LogAsync(userId, "Create", "Category", null, $"Created category '{dto.Name}'");
        
        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };
    }

    public async Task<CategoryResponseDto> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            throw new NotFoundException($"Category with ID {id} not found.");

        return new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            })
            .ToListAsync();
    }
}
