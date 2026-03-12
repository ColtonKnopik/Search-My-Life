using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntriesController : ControllerBase
{
    private readonly IJournalService _journalService;

    public EntriesController(IJournalService journalService)
    {
        _journalService = journalService;
    }

    private Guid GetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.Parse(sub!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var entries = await _journalService.GetAllAsync(userId);
        return Ok(entries);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var entry = await _journalService.GetByIdAsync(id, userId);

        if (entry is null)
            return NotFound(new { message = "Entry not found." });

        return Ok(entry);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntryRequest request)
    {
        try
        {
            var userId = GetUserId();
            var entry = await _journalService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }
        catch (DbUpdateException)
        {
            return Unauthorized(new { message = "Your session is no longer valid. Please log in again." });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEntryRequest request)
    {
        try
        {
            var userId = GetUserId();
            var entry = await _journalService.UpdateAsync(id, userId, request);

            if (entry is null)
                return NotFound(new { message = "Entry not found." });

            return Ok(entry);
        }
        catch (DbUpdateException)
        {
            return Unauthorized(new { message = "Your session is no longer valid. Please log in again." });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var deleted = await _journalService.DeleteAsync(id, userId);

        if (!deleted)
            return NotFound(new { message = "Entry not found." });

        return NoContent();
    }
}
