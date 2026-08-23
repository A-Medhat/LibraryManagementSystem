using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Borrowings;

public class BorrowRequest
{
    [Required]
    public int CopyId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}
