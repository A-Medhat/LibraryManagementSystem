using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.Users;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();
        var userDtos = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            });
        }

        return userDtos;
    }

    public async Task<UserResponseDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive
        };
    }

    public async Task<UserResponseDto> CreateAsync(CreateSystemUserRequest dto, string currentUserId)
    {
        var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
        if (!roleExists)
            throw new ConflictException($"The role '{dto.Role}' does not exist.");

        var existingUser = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null)
            throw new ConflictException($"Username '{dto.UserName}' is already taken.");

        var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
            throw new ConflictException($"Email '{dto.Email}' is already taken.");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ConflictException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, dto.Role);

        await _activityLogService.LogAsync(currentUserId, "Create", "ApplicationUser", null, $"Created user '{user.UserName}' with ID '{user.Id}' and role '{dto.Role}'");
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task<UserResponseDto> UpdateAsync(string id, UpdateSystemUserRequest dto, string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (dto.Email != null && !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null && existingEmail.Id != id)
                throw new ConflictException($"Email '{dto.Email}' is already taken.");
            
            user.Email = dto.Email;
        }

        if (dto.Role != null)
        {
            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
                throw new ConflictException($"The role '{dto.Role}' does not exist.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ConflictException($"User update failed: {errors}");
        }

        await _activityLogService.LogAsync(currentUserId, "Update", "ApplicationUser", null, $"Updated user '{user.UserName}' with ID '{user.Id}'");
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task DeleteAsync(string id, string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ConflictException($"User deactivation failed: {errors}");
        }

        await _activityLogService.LogAsync(currentUserId, "Delete", "ApplicationUser", null, $"Deactivated user '{user.UserName}' with ID '{user.Id}'");
        await _context.SaveChangesAsync();
    }

}
