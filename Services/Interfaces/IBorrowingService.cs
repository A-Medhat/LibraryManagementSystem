using LibraryManagementSystem.Models.DTOs.Borrowings;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IBorrowingService
{
    Task<BorrowingResponseDto> BorrowBookAsync(BorrowRequest dto, string userId);
    Task<BorrowingResponseDto> ReturnBookAsync(long transactionId, string userId);
    Task<BorrowingResponseDto?> GetTransactionByIdAsync(long transactionId);
    Task<List<BorrowingResponseDto>> GetMemberTransactionsAsync(int memberId);
}
