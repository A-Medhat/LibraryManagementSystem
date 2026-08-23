using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.Borrowings;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;

    public BorrowingController(IBorrowingService borrowingService)
    {
        _borrowingService = borrowingService;
    }

    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow([FromBody] BorrowRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var transaction = await _borrowingService.BorrowBookAsync(request, userId);
        return Ok(transaction);
    }

    [HttpPost("{id}/return")]
    public async Task<IActionResult> Return(long id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var transaction = await _borrowingService.ReturnBookAsync(id, userId);
        return Ok(transaction);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var transaction = await _borrowingService.GetTransactionByIdAsync(id);
        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetMemberTransactions(int memberId)
    {
        var transactions = await _borrowingService.GetMemberTransactionsAsync(memberId);
        return Ok(transactions);
    }
}
