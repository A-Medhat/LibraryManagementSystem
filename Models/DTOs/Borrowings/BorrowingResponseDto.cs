namespace LibraryManagementSystem.Models.DTOs.Borrowings;

public class BorrowingResponseDto
{
    public long TransactionId { get; set; }


    public int CopyId { get; set; }
    public string Barcode { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;

    public string IssuedByUserName { get; set; } = string.Empty;
    public string? ReturnedToUserName { get; set; }

    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
