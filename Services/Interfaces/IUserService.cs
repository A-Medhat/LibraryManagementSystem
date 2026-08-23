using LibraryManagementSystem.Models.DTOs.Users;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(string id);
    Task<UserResponseDto> CreateAsync(CreateSystemUserRequest dto, string currentUserId);
    Task<UserResponseDto> UpdateAsync(string id, UpdateSystemUserRequest dto, string currentUserId);
    Task DeleteAsync(string id, string currentUserId);
}
