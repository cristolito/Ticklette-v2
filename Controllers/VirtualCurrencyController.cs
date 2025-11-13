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
public class VirtualCurrencyController : ControllerBase
{
    private readonly VirtualCurrencyService _currencyService;
    private readonly UserManager<User> _userManager;

    public VirtualCurrencyController(VirtualCurrencyService currencyService, UserManager<User> userManager)
    {
        _currencyService = currencyService;
        _userManager = userManager;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var balance = await _currencyService.GetBalanceAsync(userId);
        return Ok(balance);
    }

    [HttpPost("add-balance")]
    public async Task<IActionResult> AddBalance([FromBody] decimal amount)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var result = await _currencyService.AddBalanceAsync(userId, amount);
        if (result == null) return BadRequest("Failed to add balance");

        return Ok(result);
    }

    [HttpPut("balance")]
    [Authorize(Roles = "Admin")] // Solo administradores pueden setear balance directamente
    public async Task<IActionResult> UpdateBalance(string userId, [FromBody] UpdateVirtualCurrencyRequest request)
    {
        var result = await _currencyService.UpdateBalanceAsync(userId, request);
        if (result == null) return BadRequest("User not found");

        return Ok(result);
    }
}