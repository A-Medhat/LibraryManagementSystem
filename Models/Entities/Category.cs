using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;

/// <summary>
/// Genre / subject classification for books.
/// Self-referencing: a Category can have a ParentCategory,
/// enabling a two-level hierarchy (e.g. Fiction → Science Fiction).
/// ParentCategoryId = null means it's a top-level category.
/// 
/// The self-referencing FK relationship and its cascade behavior
/// (NoAction to avoid multiple cascade paths) is configured via
/// Fluent API in ApplicationDbContext.OnModelCreating.
/// </summary>
public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }

    public ICollection<BookCategory> BookCategories { get; set; }
}
