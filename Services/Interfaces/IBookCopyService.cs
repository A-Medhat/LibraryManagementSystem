using LibraryManagementSystem.Models.DTOs.BookCopies;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IBookCopyService
{
    Task<BookCopyResponseDto> CreateAsync(CreateBookCopyRequest dto, string userId);
    Task<BookCopyResponseDto> GetByIdAsync(int id);
    Task<List<BookCopyResponseDto>> GetCopiesForBookAsync(int bookId);
}
