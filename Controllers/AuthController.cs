using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
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
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        // Generar token manualmente (simplificado para el ejemplo)
        var token = await GenerateJwtToken(user);
        
        var userResponse = user.ToUserResponse();
        var loginResponse = new LoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(2),
            User = userResponse
        };

        return Ok(loginResponse);
    }

    [HttpGet("profile")]
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

    private async Task<string> GenerateJwtToken(User user)
    {
        // Implementación simplificada - en producción usar JWT real
        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
            new(System.Security.Claims.ClaimTypes.Email, user.Email!),
            new("customRole", user.CustomRole.ToString())
        };

        // Agregar roles si es necesario
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role));
        }

        // En producción, generar JWT real aquí
        return $"mock-token-{user.Id}-{DateTime.UtcNow.Ticks}";
    }
}