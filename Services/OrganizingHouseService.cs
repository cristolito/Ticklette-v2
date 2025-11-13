using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
using Ticklette.DTOs.Mappers;

namespace Ticklette.Services;

public class OrganizingHouseService
{
    private readonly TickletteContext _context;

    public OrganizingHouseService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Obtener todas las houses de un organizador
    public async Task<List<OrganizingHouseResponse>> GetHousesByOrganizerAsync(int organizerId)
    {
        return await _context.OrganizingHouses
            .Where(oh => oh.OrganizerId == organizerId)
            .Select(oh => oh.ToOrganizingHouseResponse())
            .ToListAsync();
    }

    // ✅ Obtener house por ID
    public async Task<OrganizingHouseResponse?> GetHouseByIdAsync(int houseId)
    {
        var house = await _context.OrganizingHouses
            .Include(oh => oh.Events)
            .FirstOrDefaultAsync(oh => oh.OrganizingHouseId == houseId);
        
        return house?.ToOrganizingHouseResponse();
    }

    // ✅ Crear nueva house para organizador
    public async Task<OrganizingHouseResponse> CreateHouseAsync(CreateOrganizingHouseRequest request, int organizerId)
    {
        var house = request.ToOrganizingHouseEntity(organizerId);
        
        _context.OrganizingHouses.Add(house);
        await _context.SaveChangesAsync();

        return house.ToOrganizingHouseResponse();
    }

    // ✅ Actualizar house
    public async Task<OrganizingHouseResponse?> UpdateHouseAsync(int houseId, CreateOrganizingHouseRequest request)
    {
        var house = await _context.OrganizingHouses.FindAsync(houseId);
        if (house == null) return null;

        house.Name = request.Name;
        house.Address = request.Address;
        house.Contact = request.Contact;
        house.TaxData = request.TaxData;

        await _context.SaveChangesAsync();
        return house.ToOrganizingHouseResponse();
    }

    // ✅ Eliminar house (solo si no tiene eventos)
    public async Task<bool> DeleteHouseAsync(int houseId)
    {
        var house = await _context.OrganizingHouses
            .Include(oh => oh.Events)
            .FirstOrDefaultAsync(oh => oh.OrganizingHouseId == houseId);
        
        if (house == null || house.Events.Any()) 
            return false;

        _context.OrganizingHouses.Remove(house);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ Obtener organizer ID por user ID
    public async Task<int?> GetOrganizerIdByUserIdAsync(string userId)
    {
        var organizer = await _context.Organizers
            .FirstOrDefaultAsync(o => o.UserId == userId);
        
        return organizer?.OrganizerId;
    }
}