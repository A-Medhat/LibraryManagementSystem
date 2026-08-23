using LibraryManagementSystem.Common.Exceptions;
using LibraryManagementSystem.Common.Responses;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.DTOs.Books;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public BookService(ApplicationDbContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }


    public async Task<PagedResult<BookResponseDto>> GetAllAsync(BookSearchDto search)
    {
        var query = _context.Books
            .Where(b => !b.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search.Title))
            query = query.Where(b => b.Title.Contains(search.Title));

        if (!string.IsNullOrWhiteSpace(search.AuthorName))
            query = query.Where(b => b.BookAuthors.Any(ba =>
                ba.Author.FirstName.Contains(search.AuthorName) ||
                ba.Author.LastName.Contains(search.AuthorName)));

        if (!string.IsNullOrWhiteSpace(search.CategoryName))
            query = query.Where(b => b.BookCategories.Any(bc =>
                bc.Category.Name.Contains(search.CategoryName)));

        if (!string.IsNullOrWhiteSpace(search.CopyStatus))
            query = query.Where(b => b.Copies.Any(c => c.Status == search.CopyStatus));


        var totalCount = await query.CountAsync();
        var pageSize = Math.Clamp(search.PageSize, 1, 50);
        var pageNumber = Math.Max(search.PageNumber, 1);

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookResponseDto
            {
                BookId = b.BookId,
                ISBN = b.ISBN,
                Title = b.Title,
                Edition = b.Edition,
                Language = b.Language,
                PublicationYear = b.PublicationYear,
                CoverImageUrl = b.CoverImageUrl,
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher.Name,
                Authors = b.BookAuthors
                                   .Select(ba => ba.Author.FirstName + " " + ba.Author.LastName)
                                   .ToList(),
                Categories = b.BookCategories
                                   .Select(bc => bc.Category.Name)
                                   .ToList(),
                AvailableCopies = b.Copies.Count(c => c.Status == CopyStatus.Available),
                TotalCopies = b.Copies.Count,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<BookResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BookResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Where(b => b.BookId == id && !b.IsDeleted)
            .AsNoTracking()
            .Select(b => new BookResponseDto
            {
                BookId = b.BookId,
                ISBN = b.ISBN,
                Title = b.Title,
                Edition = b.Edition,
                Language = b.Language,
                PublicationYear = b.PublicationYear,
                Summary = b.Summary,
                CoverImageUrl = b.CoverImageUrl,
                PageCount = b.PageCount,
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher.Name,
                Authors = b.BookAuthors
                                   .Select(ba => ba.Author.FirstName + " " + ba.Author.LastName)
                                   .ToList(),
                Categories = b.BookCategories
                                   .Select(bc => bc.Category.Name)
                                   .ToList(),
                AvailableCopies = b.Copies.Count(c => c.Status == CopyStatus.Available),
                TotalCopies = b.Copies.Count,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }


    public async Task<BookResponseDto> CreateAsync(CreateBookRequest dto, string userId)
    {


        var isbnTaken = await _context.Books
            .AnyAsync(b => b.ISBN == dto.ISBN && !b.IsDeleted);
        if (isbnTaken)
            throw new ConflictException($"A book with ISBN '{dto.ISBN}' already exists.");

        var publisherExists = await _context.Publishers
            .AnyAsync(p => p.PublisherId == dto.PublisherId);
        if (!publisherExists)
            throw new NotFoundException("Publisher not found.");
        dto.AuthorIds = dto.AuthorIds.Distinct().ToList();
        dto.CategoryIds = dto.CategoryIds.Distinct().ToList();

        //Required at least one in DTO
        if (dto.AuthorIds.Count > 0)
        {
           /* 
              SELECT COUNT(*)
              FROM Authors
              WHERE AuthorId IN(10, 20, 30);
           */
            var foundCount = await _context.Authors
                .CountAsync(a => dto.AuthorIds.Contains(a.AuthorId));
            
            if (foundCount != dto.AuthorIds.Count)
                throw new NotFoundException("One or more authors were not found.");
        }
        //Required at least one in DTO
        if (dto.CategoryIds.Count > 0)
        {
            var foundCount = await _context.Categories
                .CountAsync(c => dto.CategoryIds.Contains(c.CategoryId));
            if (foundCount != dto.CategoryIds.Count)
                throw new NotFoundException("One or more categories were not found.");
        }

        var book = new Book
        {
            ISBN = dto.ISBN,
            Title = dto.Title,
            Edition = dto.Edition,
            Language = dto.Language,
            PublicationYear = dto.PublicationYear,
            Summary = dto.Summary,
            CoverImageUrl = dto.CoverImageUrl,
            PageCount = dto.PageCount,
            PublisherId = dto.PublisherId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var authorId in dto.AuthorIds)
            book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });

        foreach (var categoryId in dto.CategoryIds)
            book.BookCategories.Add(new BookCategory { CategoryId = categoryId });

        _context.Books.Add(book);
        await _activityLogService.LogAsync(userId, "Create", "Book", null, $"Created book '{dto.Title}' (ISBN: {dto.ISBN})");
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(book.BookId))!;
    }



    public async Task<BookResponseDto> UpdateAsync(int id, UpdateBookRequest dto, string userId)
    {
    

        var book = await _context.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.BookId == id && !b.IsDeleted);

        if (book == null)
            throw new NotFoundException("Book not found.");


        if (dto.ISBN != null && dto.ISBN != book.ISBN)
        {
            var isbnTaken = await _context.Books
                .AnyAsync(b => b.ISBN == dto.ISBN && !b.IsDeleted && b.BookId != id);
            if (isbnTaken)
                throw new ConflictException($"A book with ISBN '{dto.ISBN}' already exists.");
        }

        if (dto.PublisherId.HasValue)
        {
            var publisherExists = await _context.Publishers
                .AnyAsync(p => p.PublisherId == dto.PublisherId.Value);
            if (!publisherExists)
                throw new NotFoundException("Publisher not found.");
        }

        if (dto.ISBN != null) book.ISBN = dto.ISBN;
        if (dto.Title != null) book.Title = dto.Title;
        if (dto.Edition != null) book.Edition = dto.Edition;
        if (dto.Language != null) book.Language = dto.Language;
        if (dto.PublicationYear != null) book.PublicationYear = dto.PublicationYear;
        if (dto.Summary != null) book.Summary = dto.Summary;
        if (dto.CoverImageUrl != null) book.CoverImageUrl = dto.CoverImageUrl;
        if (dto.PageCount != null) book.PageCount = dto.PageCount;
        if (dto.PublisherId != null) book.PublisherId = dto.PublisherId.Value;
        book.UpdatedAt = DateTime.UtcNow;


        if (dto.AuthorIds != null)
        {
            dto.AuthorIds = dto.AuthorIds.Distinct().ToList();
            var foundCount = await _context.Authors
                    .CountAsync(a => dto.AuthorIds.Contains(a.AuthorId));
                if (foundCount != dto.AuthorIds.Count)
                    throw new NotFoundException("One or more authors were not found.");
            
            book.BookAuthors.Clear();
            foreach (var authorId in dto.AuthorIds)
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId, BookId = id });
        }

        if (dto.CategoryIds != null)
        {
            dto.CategoryIds = dto.CategoryIds.Distinct().ToList();
            var foundCount = await _context.Categories
                    .CountAsync(c => dto.CategoryIds.Contains(c.CategoryId));
                if (foundCount != dto.CategoryIds.Count)
                    throw new NotFoundException("One or more categories were not found.");
            
            book.BookCategories.Clear();
            foreach (var categoryId in dto.CategoryIds)
                book.BookCategories.Add(new BookCategory { CategoryId = categoryId, BookId = id });
        }

        await _activityLogService.LogAsync(userId, "Update", "Book", id, $"Updated book '{book.Title}'");
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }


    public async Task DeleteAsync(int id, string userId)
    {
        var book = await _context.Books
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.BookId == id && !b.IsDeleted);

        if (book == null)
            throw new NotFoundException("Book not found.");

        if (book.Copies.Any(c => c.Status == CopyStatus.Borrowed))
            throw new ConflictException("Cannot delete this book while one or more copies are currently borrowed.");

        book.IsDeleted = true;
        book.UpdatedAt = DateTime.UtcNow;

        await _activityLogService.LogAsync(userId, "Delete", "Book", id,
            $"Soft-deleted book '{book.Title}");

        await _context.SaveChangesAsync();
    }


}
