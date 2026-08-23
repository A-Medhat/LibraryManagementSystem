using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTOs.Users;

public class UserUpdateDto
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    public bool? IsActive { get; set; }
    public string? Role { get; set; }
}
