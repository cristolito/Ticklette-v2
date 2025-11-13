using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
using Ticklette.Services;

namespace Ticklette.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Organizer")]
public class EntriesController : ControllerBase
{
    private readonly EntryService _entryService;
    private readonly UserManager<User> _userManager;

    public EntriesController(EntryService entryService, UserManager<User> userManager)
    {
        _entryService = entryService;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEntry([FromBody] CreateEntryRequest request)
    {
        var entry = await _entryService.CreateEntryAsync(request);
        if (entry == null) 
            return BadRequest("Invalid ticket or already used");

        return Ok(entry);
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetEventEntries(int eventId)
    {
        var entries = await _entryService.GetEntriesByEventAsync(eventId);
        return Ok(entries);
    }

    [HttpGet("check-ticket/{ticketId}")]
    public async Task<IActionResult> CheckTicket(int ticketId)
    {
        var canEnter = await _entryService.CanTicketEnterAsync(ticketId);
        return Ok(new { canEnter });
    }
}