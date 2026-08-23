using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;


[Index(nameof(ISBN), IsUnique = true)]
public class Book
{
    public int BookId { get; set; }

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


    public int PublisherId { get; set; }


    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Publisher Publisher { get; set; }
    public ICollection<BookCopy> Copies { get; set; } = [];
    public ICollection<BookAuthor> BookAuthors { get; set; } = [];
    public ICollection<BookCategory> BookCategories { get; set; } = [];
}
