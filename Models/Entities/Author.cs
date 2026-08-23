using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;

/// <summary>
/// A book author. One Author can write many Books (M:M via BookAuthor join).
/// </summary>
public class Author
{
    public int AuthorId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = [];
}
