using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Requests;
using Ticklette.Services;

namespace Ticklette.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(AuthService authService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _authService = authService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register/attendee")]
    public async Task<IActionResult> RegisterAttendee([FromBody] CreateAttendeeRequest request)
    {
        var result = await _authService.RegisterAttendeeAsync(request);
        
        if (result.Succeeded)
            return Ok(new { message = "Attendee registered successfully" });
        
        return BadRequest(result.Errors);
    }

    [HttpPost("register/organizer")]
    public async Task<IActionResult> RegisterOrganizer([FromBody] CreateOrganizerRequest request)
    {
        var result = await _authService.RegisterOrganizerAsync(request);
        
        if (result.Succeeded)
            return Ok(new { message = "Organizer registered successfully" });
        
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized("Invalid credentials");

        return Ok(result);
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        // En un escenario real, validarías el token de Google aquí
        // Por simplicidad, asumimos que el frontend ya validó el token
        
        var result = await _authService.HandleGoogleLoginAsync(
            request.Email, 
            request.FirstName, 
            request.LastName);
            
        if (result == null)
            return BadRequest("Failed to authenticate with Google");

        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize] // Requiere autenticación
    public async Task<IActionResult> GetProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var profile = await _authService.GetUserProfileAsync(userId);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }
}
