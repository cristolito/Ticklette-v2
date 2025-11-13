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
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly UserManager<User> _userManager;

    public TicketsController(TicketService ticketService, UserManager<User> userManager)
    {
        _ticketService = ticketService;
        _userManager = userManager;
    }

    [HttpPost("purchase")]
    [Authorize]
    public async Task<IActionResult> PurchaseTickets([FromBody] PurchaseTicketRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        try
        {
            var tickets = await _ticketService.PurchaseTicketsAsync(request, userId);
            return Ok(tickets);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("purchase-product")]
    [Authorize]
    public async Task<IActionResult> PurchaseProduct([FromBody] PurchaseProductRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        try
        {
            var sale = await _ticketService.PurchaseProductAsync(request, userId);
            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-tickets")]
    [Authorize]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var tickets = await _ticketService.GetUserTicketsAsync(userId);
        return Ok(tickets);
    }
}