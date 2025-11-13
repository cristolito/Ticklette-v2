using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class EntryService
{
    private readonly TickletteContext _context;

    public EntryService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Registrar entrada (usar ticket)
    public async Task<EntryResponse?> CreateEntryAsync(CreateEntryRequest request)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Entry)
            .FirstOrDefaultAsync(t => t.TicketId == request.TicketId);

        if (ticket == null || ticket.Status != "Active" || ticket.Entry != null)
            return null;

        // Crear entry
        var entry = request.ToEntryEntity();
        _context.Entries.Add(entry);

        // Marcar ticket como usado
        ticket.Status = "Used";

        await _context.SaveChangesAsync();
        return entry.ToEntryResponse();
    }

    // ✅ Obtener entradas de un evento
    public async Task<List<EntryResponse>> GetEntriesByEventAsync(int eventId)
    {
        return await _context.Entries
            .Where(e => e.Ticket!.TicketType!.EventId == eventId)
            .Select(e => e.ToEntryResponse())
            .ToListAsync();
    }

    // ✅ Verificar si un ticket puede entrar
    public async Task<bool> CanTicketEnterAsync(int ticketId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Entry)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);

        return ticket != null && ticket.Status == "Active" && ticket.Entry == null;
    }
}