using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.BookCopies;

public class CreateBookCopyRequest
{
    [Required]
    public int BookId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Barcode { get; set; } = string.Empty;
}
