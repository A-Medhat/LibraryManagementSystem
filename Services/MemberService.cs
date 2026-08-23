using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.Members;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public MemberService(ApplicationDbContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<List<MemberResponseDto>> GetAllAsync()
    {

        return await _context.Members
            .AsNoTracking()
            .OrderByDescending(m => m.JoinDate)
            .Select(m => new MemberResponseDto
            {
                MemberId = m.MemberId,
                MembershipNumber = m.MembershipNumber,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                Phone = m.Phone,
                Address = m.Address,
                JoinDate = m.JoinDate,
                ExpiryDate = m.ExpiryDate,
                Status = m.Status,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<MemberResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Members
            .AsNoTracking()
            .Where(m => m.MemberId == id)
            .Select(m => new MemberResponseDto
            {
                MemberId = m.MemberId,
                MembershipNumber = m.MembershipNumber,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                Phone = m.Phone,
                Address = m.Address,
                JoinDate = m.JoinDate,
                ExpiryDate = m.ExpiryDate,
                Status = m.Status,
                CreatedAt = m.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<MemberResponseDto> CreateAsync(CreateMemberRequest dto, string userId)
    {
        var emailTaken = await _context.Members
            .AnyAsync(m => m.Email.ToLower() == dto.Email.ToLower());
        if (emailTaken)
            throw new ConflictException($"A member with Email '{dto.Email}' already exists.");

        var maxMemberId = await _context.Members.MaxAsync(m => (int?)m.MemberId) ?? 0;
        var generatedMembershipNumber = $"MEM-{(maxMemberId + 1):D6}";

        var member = new Member
        {
            MembershipNumber = generatedMembershipNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            Status = MemberStatus.Active,
            JoinDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Members.Add(member);

        await _activityLogService.LogAsync(userId, "Create", "Member", null, $"Created member '{member.FirstName} {member.LastName}' (Membership: {member.MembershipNumber})");

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(member.MemberId))!;
    }

    public async Task<MemberResponseDto> UpdateAsync(int id, UpdateMemberRequest dto, string userId)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.MemberId == id);

        if (member == null)
            throw new NotFoundException("Member not found.");

        if (dto.Email != null && !string.Equals(dto.Email, member.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await _context.Members
                .AnyAsync(m => m.Email.ToLower() == dto.Email.ToLower() && m.MemberId != id);
            if (emailTaken)
                throw new ConflictException($"A member with Email '{dto.Email}' already exists.");
        }

        if (dto.FirstName != null) member.FirstName = dto.FirstName;
        if (dto.LastName != null) member.LastName = dto.LastName;
        if (dto.Email != null) member.Email = dto.Email;
        if (dto.Phone != null) member.Phone = dto.Phone;
        if (dto.Address != null) member.Address = dto.Address;
        if (dto.Status != null) member.Status = dto.Status;

        member.UpdatedAt = DateTime.UtcNow;

        await _activityLogService.LogAsync(userId, "Update", "Member", id, $"Updated member '{member.FirstName} {member.LastName}'");

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var member = await _context.Members
            .Include(m => m.Transactions)
            .FirstOrDefaultAsync(m => m.MemberId == id);

        if (member == null)
            throw new NotFoundException("Member not found.");

        if (member.Transactions.Any(t => t.Status == TransactionStatus.Active))
            throw new ConflictException("Cannot deactivate this member while they have active borrowing transactions.");

        member.Status = MemberStatus.Suspended;
        member.UpdatedAt = DateTime.UtcNow;

        await _activityLogService.LogAsync(userId, "Delete", "Member", id, $"Soft-deleted (suspended) member '{member.FirstName} {member.LastName}'");

        await _context.SaveChangesAsync();
    }

}
