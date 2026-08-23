using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;


[Index(nameof(Barcode), IsUnique = true)]
public class BookCopy
{
    [Key]
    public int CopyId { get; set; }


    public int BookId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Barcode { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = CopyStatus.Available;

    [MaxLength(100)]
    public string? ShelfLocation { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public decimal? PurchasePrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public Book Book { get; set; }
    public ICollection<BorrowingTransaction> Transactions { get; set; }
}


public static class CopyStatus
{
    public const string Available = "Available";
    public const string Borrowed = "Borrowed";
    public const string Lost = "Lost";
}
