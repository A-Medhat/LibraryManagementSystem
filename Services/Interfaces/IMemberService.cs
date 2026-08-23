using LibraryManagementSystem.Models.DTOs.Members;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IMemberService
{
    Task<List<MemberResponseDto>> GetAllAsync();
    Task<MemberResponseDto?> GetByIdAsync(int id);
    Task<MemberResponseDto> CreateAsync(CreateMemberRequest dto, string userId);
    Task<MemberResponseDto> UpdateAsync(int id, UpdateMemberRequest dto, string userId);
    Task DeleteAsync(int id, string userId);
}
