using LibraryManagementSystem.Common.Responses;
using LibraryManagementSystem.Models.DTOs.Books;

namespace LibraryManagementSystem.Services.Interfaces;

public interface IBookService
{
    Task<PagedResult<BookResponseDto>> GetAllAsync(BookSearchDto search);
    Task<BookResponseDto?> GetByIdAsync(int id);
    Task<BookResponseDto> CreateAsync(CreateBookRequest dto, string userId);
    Task<BookResponseDto> UpdateAsync(int id, UpdateBookRequest dto, string userId);
    Task DeleteAsync(int id, string userId);
}
