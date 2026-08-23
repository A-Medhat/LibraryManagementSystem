using System.Security.Claims;
using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.DTOs.Users;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Administrator)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSystemUserRequest request)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.CreateAsync(request, currentUserId);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSystemUserRequest request)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.UpdateAsync(id, request, currentUserId);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.DeleteAsync(id, currentUserId);
        return NoContent();
    }
}
