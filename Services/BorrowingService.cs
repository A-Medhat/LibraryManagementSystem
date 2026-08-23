using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.Borrowings;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class BorrowingService : IBorrowingService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public BorrowingService(ApplicationDbContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<BorrowingResponseDto> BorrowBookAsync(BorrowRequest dto, string userId)
    {   
        var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == dto.MemberId);
        if (member == null)
            throw new NotFoundException("Member not found.");

        if (member.Status != MemberStatus.Active)
            throw new ConflictException("Member is not active and cannot borrow books.");

        var copy = await _context.BookCopies.FirstOrDefaultAsync(c => c.CopyId == dto.CopyId);
        if (copy == null)
            throw new NotFoundException("Book copy not found.");

        if (copy.Status != CopyStatus.Available)
            throw new ConflictException("Book copy is not currently available for borrowing.");

        if (dto.DueDate <= DateTime.UtcNow)
            throw new ConflictException("Due date must be in the future.");

        var transaction = new BorrowingTransaction
        {
            CopyId = dto.CopyId,
            MemberId = dto.MemberId,
            IssuedByUserId = userId,
            DueDate = dto.DueDate,
            Status = TransactionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            BorrowedAt = DateTime.UtcNow
        };

        copy.Status = CopyStatus.Borrowed;

        _context.BorrowingTransactions.Add(transaction);

        await _activityLogService.LogAsync(userId, "Borrow", "BookCopy", copy.CopyId, $"Borrowed copy {copy.Barcode} to member {member.MembershipNumber}");

        await _context.SaveChangesAsync();

        return (await GetTransactionByIdAsync(transaction.TransactionId))!;
    }

    public async Task<BorrowingResponseDto> ReturnBookAsync(long transactionId, string userId)
    {
        var transaction = await _context.BorrowingTransactions
            .Include(t => t.Copy)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

        if (transaction == null)
            throw new NotFoundException("Borrowing transaction not found.");

        if (transaction.Status == TransactionStatus.Returned)
            throw new ConflictException("This book copy has already been returned.");

        transaction.ReturnDate = DateTime.UtcNow;
        transaction.ReturnedToUserId = userId;
        transaction.Status = TransactionStatus.Returned;

        transaction.Copy.Status = CopyStatus.Available;

        await _activityLogService.LogAsync(userId, "Return", "BookCopy", transaction.CopyId, $"Returned copy {transaction.Copy.Barcode}");

        await _context.SaveChangesAsync();

        return (await GetTransactionByIdAsync(transaction.TransactionId))!;
    }

    public async Task<BorrowingResponseDto?> GetTransactionByIdAsync(long transactionId)
    {
        return await _context.BorrowingTransactions
            .AsNoTracking()
            .Where(t => t.TransactionId == transactionId)
            .Select(t => new BorrowingResponseDto
            {
                TransactionId = t.TransactionId,
                CopyId = t.CopyId,
                Barcode = t.Copy.Barcode,
                BookTitle = t.Copy.Book.Title,
                MemberId = t.MemberId,
                MemberName = t.Member.FirstName + " " + t.Member.LastName,
                MembershipNumber = t.Member.MembershipNumber,
                IssuedByUserName = t.IssuedByUser.UserName!,
                ReturnedToUserName = t.ReturnedToUser != null ? t.ReturnedToUser.UserName : null,
                BorrowedAt = t.BorrowedAt,
                DueDate = t.DueDate,
                ReturnDate = t.ReturnDate,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<BorrowingResponseDto>> GetMemberTransactionsAsync(int memberId)
    {
        return await _context.BorrowingTransactions
            .AsNoTracking()
            .Where(t => t.MemberId == memberId)
            .OrderByDescending(t => t.BorrowedAt)
            .Select(t => new BorrowingResponseDto
            {
                TransactionId = t.TransactionId,
                CopyId = t.CopyId,
                Barcode = t.Copy.Barcode,
                BookTitle = t.Copy.Book.Title,
                MemberId = t.MemberId,
                MemberName = t.Member.FirstName + " " + t.Member.LastName,
                MembershipNumber = t.Member.MembershipNumber,
                IssuedByUserName = t.IssuedByUser.UserName!,
                ReturnedToUserName = t.ReturnedToUser != null ? t.ReturnedToUser.UserName : null,
                BorrowedAt = t.BorrowedAt,
                DueDate = t.DueDate,
                ReturnDate = t.ReturnDate,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

}
