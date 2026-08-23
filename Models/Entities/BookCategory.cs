namespace LibraryManagementSystem.Models.Entities;


public class BookCategory
{
    public int CategoryId { get; set; }
    public int BookId { get; set; }

    public Category Category { get; set; }
    public Book Book { get; set; }

}