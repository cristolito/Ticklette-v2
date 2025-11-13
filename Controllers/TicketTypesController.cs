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
public class TicketTypesController : ControllerBase
{
    private readonly TicketTypeService _ticketTypeService;
    private readonly EventService _eventService;
    private readonly OrganizingHouseService _houseService;
    private readonly UserManager<User> _userManager;

    public TicketTypesController(
        TicketTypeService ticketTypeService, 
        EventService eventService,
        OrganizingHouseService houseService,
        UserManager<User> userManager)
    {
        _ticketTypeService = ticketTypeService;
        _eventService = eventService;
        _houseService = houseService;
        _userManager = userManager;
    }

    [HttpGet("event/{eventId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTicketTypesByEvent(int eventId)
    {
        var ticketTypes = await _ticketTypeService.GetTicketTypesByEventAsync(eventId);
        return Ok(ticketTypes);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTicketType(int id)
    {
        var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
        if (ticketType == null) return NotFound();
        return Ok(ticketType);
    }

    [HttpPost("event/{eventId}")]
    public async Task<IActionResult> CreateTicketType(int eventId, [FromBody] CreateTicketTypeRequest request)
    {
        // Verificar permisos sobre el evento
        var eventEntity = await _eventService.GetEventByIdAsync(eventId);
        if (eventEntity == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        
        if (organizerId == null || house?.OrganizerId != organizerId) 
            return Forbid();

        var ticketType = await _ticketTypeService.CreateTicketTypeAsync(request, eventId);
        return CreatedAtAction(nameof(GetTicketType), new { id = ticketType.TicketTypeId }, ticketType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicketType(int id, [FromBody] CreateTicketTypeRequest request)
    {
        var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
        if (ticketType == null) return NotFound();

        // Verificar permisos
        var eventEntity = await _eventService.GetEventByIdAsync(ticketType.EventId);
        if (eventEntity == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        
        if (organizerId == null || house?.OrganizerId != organizerId) 
            return Forbid();

        var updatedTicketType = await _ticketTypeService.UpdateTicketTypeAsync(id, request);
        return Ok(updatedTicketType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicketType(int id)
    {
        var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
        if (ticketType == null) return NotFound();

        // Verificar permisos
        var eventEntity = await _eventService.GetEventByIdAsync(ticketType.EventId);
        if (eventEntity == null) return NotFound();

        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        if (house == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();
        
        
        if (organizerId == null || house?.OrganizerId != organizerId) 
            return Forbid();

        var result = await _ticketTypeService.DeleteTicketTypeAsync(id);
        if (!result) return BadRequest("Cannot delete ticket type with sold tickets");

        return NoContent();
    }
}