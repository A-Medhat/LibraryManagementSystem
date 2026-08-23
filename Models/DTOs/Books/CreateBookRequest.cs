using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Books;

/// <summary>
/// Used for POST /api/books — all required fields must be present.
/// </summary>
public class CreateBookRequest
{
    [Required]
    [MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Edition { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }

    public short? PublicationYear { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    public int? PageCount { get; set; }

    [Required]
    public int PublisherId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one author is required.")]
    public List<int> AuthorIds { get; set; } = [];

    [Required]
    [MinLength(1, ErrorMessage = "At least one category is required.")]
    public List<int> CategoryIds { get; set; } = [];
}
