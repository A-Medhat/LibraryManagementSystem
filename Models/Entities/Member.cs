using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;


[Index(nameof(MembershipNumber), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Member
{
    public int MemberId { get; set; }

    [Required]
    [MaxLength(20)]
    public string MembershipNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ExpiryDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = MemberStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BorrowingTransaction> Transactions { get; set; }
}

public static class MemberStatus
{
    public const string Active = "Active";
    public const string Suspended = "Suspended";
    public const string Expired = "Expired";
}
