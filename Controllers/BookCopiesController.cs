using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.BookCopies;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyService _bookCopyService;

    public BookCopiesController(IBookCopyService bookCopyService)
    {
        _bookCopyService = bookCopyService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var copy = await _bookCopyService.GetByIdAsync(id);
        return Ok(copy);
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetCopiesForBook(int bookId)
    {
        var copies = await _bookCopyService.GetCopiesForBookAsync(bookId);
        return Ok(copies);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian},{AppRoles.Staff}")]
    public async Task<IActionResult> Create([FromBody] CreateBookCopyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var copy = await _bookCopyService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = copy.CopyId }, copy);
    }
}
