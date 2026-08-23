using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Books;

/// <summary>
/// Used for PATCH /api/books/{id} — all fields are optional.
/// The service only updates a field if the client sends a non-null value.
/// This avoids overwriting unchanged data when the client only wants to update one field.
/// </summary>
public class UpdateBookRequest
{
    [MaxLength(20)]
    public string? ISBN { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

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
    public int? PublisherId { get; set; }

    // null   → don't touch current associations
    // [1, 2] → replace with these IDs
    [MinLength(1, ErrorMessage = "At least one author is required.")]
    public List<int>? AuthorIds { get; set; }

    [MinLength(1, ErrorMessage = "At least one category is required.")]
    public List<int>? CategoryIds { get; set; }
}
