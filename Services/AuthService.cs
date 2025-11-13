using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;

namespace Ticklette.Services;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly TickletteContext _context;

    public AuthService(UserManager<User> userManager, TickletteContext context)
    {
        _userManager = userManager;
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