using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly TickletteContext _context;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, 
                      IConfiguration configuration, TickletteContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    // ✅ Registrar Attendee
    public async Task<IdentityResult> RegisterAttendeeAsync(CreateAttendeeRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CustomRole = 0 // Attendee
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (result.Succeeded)
        {
            // Crear entidad Attendee
            var attendee = request.ToAttendeeEntity(user.Id);
            _context.Attendees.Add(attendee);
            
            // Crear VirtualCurrency para el usuario
            var virtualCurrency = new VirtualCurrency
            {
                UserId = user.Id,
                Balance = 0
            };
            _context.VirtualCurrencies.Add(virtualCurrency);
            
            await _context.SaveChangesAsync();
        }

        return result;
    }

    // ✅ Registrar Organizer
    public async Task<IdentityResult> RegisterOrganizerAsync(CreateOrganizerRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CustomRole = 1 // Organizer
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (result.Succeeded)
        {
            // Crear entidad Organizer
            var organizer = request.ToOrganizerEntity(user.Id);
            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            // Crear OrganizingHouse obligatoria
            var organizingHouse = new OrganizingHouse
            {
                Name = request.OrganizingHouseName,
                Address = request.OrganizingHouseAddress,
                Contact = request.OrganizingHouseContact,
                TaxData = request.OrganizingHouseTaxData,
                OrganizerId = organizer.OrganizerId
            };
            _context.OrganizingHouses.Add(organizingHouse);
            
            // Crear VirtualCurrency para el usuario
            var virtualCurrency = new VirtualCurrency
            {
                UserId = user.Id,
                Balance = 0
            };
            _context.VirtualCurrencies.Add(virtualCurrency);
            
            await _context.SaveChangesAsync();
        }

        return result;
    }

    [HttpPost("login")]
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return null;

        var token = await GenerateJwtToken(user);
        
        return new LoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(2),
            User = user.ToUserResponse()
        };
    }

    // ✅ Login/Registro con Google
    public async Task<LoginResponse?> HandleGoogleLoginAsync(string email, string firstName, string lastName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            // Registrar nuevo usuario desde Google
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CustomRole = 0, // Por defecto como Attendee
                EmailConfirmed = true // Google ya verificó el email
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return null;

            // Crear entidad Attendee
            var attendee = new Attendee
            {
                UserId = user.Id,
                DateOfBirth = DateTime.UtcNow.AddYears(-18), // Edad por defecto
                Gender = "Not specified"
            };
            _context.Attendees.Add(attendee);

            // Crear VirtualCurrency
            var virtualCurrency = new VirtualCurrency
            {
                UserId = user.Id,
                Balance = 0
            };
            _context.VirtualCurrencies.Add(virtualCurrency);

            await _context.SaveChangesAsync();
        }

        var token = await GenerateJwtToken(user);
        
        return new LoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(2),
            User = user.ToUserResponse()
        };
    }

    // ✅ Generar JWT Token real
    private async Task<string> GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("uid", user.Id),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("customRole", user.CustomRole.ToString())
        };

        // Agregar roles de Identity si los usas
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    // ✅ Obtener perfil de usuario completo
    public async Task<object?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        if (user.CustomRole == 0) // Attendee
        {
            var attendee = await _context.Attendees
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);
            return attendee?.ToAttendeeResponse();
        }
        else // Organizer
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .Include(o => o.OrganizingHouses)
                .FirstOrDefaultAsync(o => o.UserId == userId);
            return organizer?.ToOrganizerResponse();
        }
    }
}