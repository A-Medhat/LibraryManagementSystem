namespace LibraryManagementSystem.Models.DTOs.Books;

public class BookResponseDto
{
    public int BookId { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public string? Language { get; set; }
    public short? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public int? PageCount { get; set; }

    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;


    public List<string> Authors { get; set; } = [];
    public List<string> Categories { get; set; } = [];

    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
