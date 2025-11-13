using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
using Ticklette.DTOs.Common;
using Ticklette.Services;

namespace Ticklette.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly OrganizingHouseService _houseService;
    private readonly UserManager<User> _userManager;

    public EventsController(EventService eventService, OrganizingHouseService houseService, UserManager<User> userManager)
    {
        _eventService = eventService;
        _houseService = houseService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] PagedRequest request)
    {
        var events = await _eventService.GetEventsAsync(request);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var eventEntity = await _eventService.GetEventByIdAsync(id);
        if (eventEntity == null) return NotFound();
        return Ok(eventEntity);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        // Verificar que el usuario tenga permisos sobre la house
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();

        var house = await _houseService.GetHouseByIdAsync(request.OrganizingHouseId);
        if (house == null || house.OrganizerId != organizerId) 
            return BadRequest("Invalid organizing house");

        var eventEntity = await _eventService.CreateEventAsync(request, request.OrganizingHouseId);
        return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.EventId }, eventEntity);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
    {
        var eventEntity = await _eventService.GetEventByIdAsync(id);
        if (eventEntity == null) return NotFound();

        // Verificar permisos
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        
        if (organizerId == null || house?.OrganizerId != organizerId) 
            return Forbid();

        var updatedEvent = await _eventService.UpdateEventAsync(id, request);
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventEntity = await _eventService.GetEventByIdAsync(id);
        if (eventEntity == null) return NotFound();

        // Verificar permisos
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        
        if (organizerId == null || house?.OrganizerId != organizerId) 
            return Forbid();

        var result = await _eventService.DeleteEventAsync(id);
        if (!result) return BadRequest("Cannot delete event with tickets");

        return NoContent();
    }

    [HttpGet("house/{houseId}")]
    public async Task<IActionResult> GetEventsByHouse(int houseId)
    {
        var events = await _eventService.GetEventsByHouseAsync(houseId);
        return Ok(events);
    }
}