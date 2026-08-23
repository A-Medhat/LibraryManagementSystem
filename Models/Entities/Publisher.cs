using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;


[Index(nameof(Name), IsUnique = true)]
public class Publisher
{
    public int PublisherId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }
    public ICollection<Book> Books { get; set; }
}
