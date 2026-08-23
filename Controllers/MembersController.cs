using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.Members;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{AppRoles.Administrator},{AppRoles.Librarian}")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var member = await _memberService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = member.MemberId }, member);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var member = await _memberService.UpdateAsync(id, request, userId);
        return Ok(member);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _memberService.DeleteAsync(id, userId);
        return NoContent();
    }
}
