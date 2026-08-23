using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Users;

public class UpdateSystemUserRequest
{
    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Role { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    public bool? IsActive { get; set; }
}
