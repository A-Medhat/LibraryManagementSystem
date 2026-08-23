using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.Books;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BookSearchDto search)
    {
        var books = await _bookService.GetAllAsync(search);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var book = await _bookService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = book.BookId }, book);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var book = await _bookService.UpdateAsync(id, request, userId);
        return Ok(book);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _bookService.DeleteAsync(id, userId);
        return NoContent();
    }
}
