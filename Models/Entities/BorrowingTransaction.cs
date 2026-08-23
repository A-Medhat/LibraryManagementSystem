using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;

public class BorrowingTransaction
{
    [Key]
    public long TransactionId { get; set; }

    public int CopyId { get; set; }
    public int MemberId { get; set; }

    [Required]
    //string because identinty use string GID
    public string IssuedByUserId { get; set; } = string.Empty;
    public string? ReturnedToUserId { get; set; }

    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = TransactionStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public BookCopy Copy { get; set; } = null!;
    public Member Member { get; set; } = null!;
    public ApplicationUser IssuedByUser { get; set; } = null!;
    public ApplicationUser? ReturnedToUser { get; set; }
}


public static class TransactionStatus
{
    public const string Active = "Active";
    public const string Returned = "Returned";
    public const string Overdue = "Overdue";
}
