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
[Authorize]
public class OrganizingHousesController : ControllerBase
{
    private readonly OrganizingHouseService _houseService;
    private readonly UserManager<User> _userManager;

    public OrganizingHousesController(OrganizingHouseService houseService, UserManager<User> userManager)
    {
        _houseService = houseService;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMyHouses()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();

        var houses = await _houseService.GetHousesByOrganizerAsync(organizerId.Value);
        return Ok(houses);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHouse(int id)
    {
        var house = await _houseService.GetHouseByIdAsync(id);
        if (house == null) return NotFound();

        // Verificar que el usuario sea el dueño
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId != house.OrganizerId) return Forbid();

        return Ok(house);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateHouse([FromBody] CreateOrganizingHouseRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();

        var house = await _houseService.CreateHouseAsync(request, organizerId.Value);
        return CreatedAtAction(nameof(GetHouse), new { id = house.OrganizingHouseId }, house);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateHouse(int id, [FromBody] CreateOrganizingHouseRequest request)
    {
        var house = await _houseService.GetHouseByIdAsync(id);
        if (house == null) return NotFound();

        // Verificar que el usuario sea el dueño
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId != house.OrganizerId) return Forbid();

        var updatedHouse = await _houseService.UpdateHouseAsync(id, request);
        return Ok(updatedHouse);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteHouse(int id)
    {
        var house = await _houseService.GetHouseByIdAsync(id);
        if (house == null) return NotFound();

        // Verificar que el usuario sea el dueño
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId != house.OrganizerId) return Forbid();

        var result = await _houseService.DeleteHouseAsync(id);
        if (!result) return BadRequest("Cannot delete house with events");

        return NoContent();
    }
}