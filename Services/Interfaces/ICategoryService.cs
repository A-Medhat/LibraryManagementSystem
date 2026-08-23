using LibraryManagementSystem.Models.DTOs.Categories;

namespace LibraryManagementSystem.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequest dto, string userId);
    Task<CategoryResponseDto> GetByIdAsync(int id);
    Task<List<CategoryResponseDto>> GetAllAsync();
}
