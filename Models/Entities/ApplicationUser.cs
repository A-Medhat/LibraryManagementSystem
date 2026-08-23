using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;

/// <summary>
/// Extends ASP.NET Identity's IdentityUser with library-specific fields.
/// IdentityUser already provides: Id (string/GUID), Email, UserName, PasswordHash, etc.
/// We inherit instead of creating a custom users table — Identity handles hashing,
/// lockout, concurrency stamps, and the role/claim infrastructure for free.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;


    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<BorrowingTransaction> IssuedTransactions { get; set; }
    public ICollection<BorrowingTransaction> ReturnedTransactions { get; set; }
    public ICollection<UserActivityLog> ActivityLogs { get; set; }
}
