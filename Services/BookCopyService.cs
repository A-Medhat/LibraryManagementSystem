using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.BookCopies;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class BookCopyService : IBookCopyService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public BookCopyService(ApplicationDbContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    public async Task<BookCopyResponseDto> CreateAsync(CreateBookCopyRequest dto, string userId)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.BookId == dto.BookId && !b.IsDeleted);

        if (book == null)
            throw new NotFoundException($"Book with ID {dto.BookId} not found or deleted.");

        var existingBarcode = await _context.BookCopies
            .AnyAsync(c => c.Barcode == dto.Barcode);

        if (existingBarcode)
            throw new ConflictException($"A book copy with barcode '{dto.Barcode}' already exists.");

        var copy = new BookCopy
        {
            BookId = dto.BookId,
            Barcode = dto.Barcode,
            Status = CopyStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        _context.BookCopies.Add(copy);
        await _activityLogService.LogAsync(userId, "Create", "BookCopy", null, $"Created copy '{dto.Barcode}' for Book ID {dto.BookId}");
        
        await _context.SaveChangesAsync();

        return new BookCopyResponseDto
        {
            CopyId = copy.CopyId,
            BookId = copy.BookId,
            BookTitle = book.Title,
            Barcode = copy.Barcode,
            Status = copy.Status
        };
    }

    public async Task<BookCopyResponseDto> GetByIdAsync(int id)
    {
        var copy = await _context.BookCopies
            .Include(c => c.Book)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CopyId == id);

        if (copy == null)
            throw new NotFoundException($"Book copy with ID {id} not found.");

        return new BookCopyResponseDto
        {
            CopyId = copy.CopyId,
            BookId = copy.BookId,
            BookTitle = copy.Book.Title,
            Barcode = copy.Barcode,
            Status = copy.Status
        };
    }

    public async Task<List<BookCopyResponseDto>> GetCopiesForBookAsync(int bookId)
    {
        var bookExists = await _context.Books
            .AnyAsync(b => b.BookId == bookId && !b.IsDeleted);

        if (!bookExists)
            throw new NotFoundException($"Book with ID {bookId} not found or deleted.");

        return await _context.BookCopies
            .Where(c => c.BookId == bookId)
            .OrderBy(c => c.Barcode)
            .AsNoTracking()
            .Select(c => new BookCopyResponseDto
            {
                CopyId = c.CopyId,
                BookId = c.BookId,
                BookTitle = c.Book.Title,
                Barcode = c.Barcode,
                Status = c.Status
            })
            .ToListAsync();
    }
}
