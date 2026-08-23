using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
}
