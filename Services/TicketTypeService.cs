using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class TicketTypeService
{
    private readonly TickletteContext _context;

    public TicketTypeService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Obtener tipos de ticket de un evento
    public async Task<List<TicketTypeResponse>> GetTicketTypesByEventAsync(int eventId)
    {
        return await _context.TicketTypes
            .Where(tt => tt.EventId == eventId)
            .Select(tt => tt.ToTicketTypeResponse())
            .ToListAsync();
    }

    // ✅ Obtener tipo de ticket por ID
    public async Task<TicketTypeResponse?> GetTicketTypeByIdAsync(int ticketTypeId)
    {
        var ticketType = await _context.TicketTypes
            .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
        
        return ticketType?.ToTicketTypeResponse();
    }

    // ✅ Crear tipo de ticket
    public async Task<TicketTypeResponse> CreateTicketTypeAsync(CreateTicketTypeRequest request, int eventId)
    {
        var ticketType = request.ToTicketTypeEntity(eventId);
        
        _context.TicketTypes.Add(ticketType);
        await _context.SaveChangesAsync();

        return ticketType.ToTicketTypeResponse();
    }

    // ✅ Actualizar tipo de ticket
    public async Task<TicketTypeResponse?> UpdateTicketTypeAsync(int ticketTypeId, CreateTicketTypeRequest request)
    {
        var ticketType = await _context.TicketTypes.FindAsync(ticketTypeId);
        if (ticketType == null) return null;

        ticketType.Name = request.Name;
        ticketType.Description = request.Description;
        ticketType.Price = request.Price;
        ticketType.AvailableQuantity = request.AvailableQuantity;

        await _context.SaveChangesAsync();
        return ticketType.ToTicketTypeResponse();
    }

    // ✅ Eliminar tipo de ticket (solo si no hay tickets vendidos)
    public async Task<bool> DeleteTicketTypeAsync(int ticketTypeId)
    {
        var ticketType = await _context.TicketTypes
            .Include(tt => tt.Tickets)
            .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
        
        if (ticketType == null || ticketType.Tickets.Any())
            return false;

        _context.TicketTypes.Remove(ticketType);
        await _context.SaveChangesAsync();
        return true;
    }
}