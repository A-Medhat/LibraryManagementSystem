namespace LibraryManagementSystem.Models.DTOs.Books;

/// <summary>
/// Query parameters for GET /api/books
/// Bound from query string: ?title=clean&amp;authorName=martin&amp;pageNumber=1&amp;pageSize=10
/// </summary>
public class BookSearchDto
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }

    /// <summary>Filter by copy availability: Available | Borrowed | Lost</summary>
    public string? CopyStatus { get; set; }

    public int PageNumber { get; set; } = 1;

    // Capped at 50 in the service — prevents clients from fetching everything at once
    public int PageSize { get; set; } = 10;
}
