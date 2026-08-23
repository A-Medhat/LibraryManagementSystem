using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Members;

public class UpdateMemberRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }
}
