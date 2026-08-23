using LibraryManagementSystem.Models.DTOs.Users;

namespace LibraryManagementSystem.Models.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserResponseDto User { get; set; } = default!;
}
