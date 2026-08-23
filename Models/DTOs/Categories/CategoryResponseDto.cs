namespace LibraryManagementSystem.Models.DTOs.Categories;

public class CategoryResponseDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
}
