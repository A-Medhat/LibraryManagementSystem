namespace LibraryManagementSystem.Models.DTOs.BookCopies;

public class BookCopyResponseDto
{
    public int CopyId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
