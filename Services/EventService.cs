using Microsoft.EntityFrameworkCore;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
using Ticklette.DTOs.Common;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Mappers;

namespace Ticklette.Services;

public class EventService
{
    private readonly TickletteContext _context;

    public EventService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Obtener eventos con paginación y filtros
    public async Task<PagedResponse<EventResponse>> GetEventsAsync(PagedRequest request)
    {
        var query = _context.Events
            .Include(e => e.OrganizingHouse)
            .AsQueryable();

        // Búsqueda
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(e => 
                e.Name.Contains(request.Search) || 
                e.Description.Contains(request.Search) ||
                e.Location.Contains(request.Search));
        }

        // Ordenamiento
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDesc ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
            "date" => request.SortDesc ? query.OrderByDescending(e => e.DateTime) : query.OrderBy(e => e.DateTime),
            "location" => request.SortDesc ? query.OrderByDescending(e => e.Location) : query.OrderBy(e => e.Location),
            _ => request.SortDesc ? query.OrderByDescending(e => e.EventId) : query.OrderBy(e => e.EventId)
        };

        var totalCount = await query.CountAsync();
        
        var events = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => e.ToEventResponse())
            .ToListAsync();

        return new PagedResponse<EventResponse>
        {
            Items = events,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    // ✅ Obtener eventos de una house específica
    public async Task<List<EventResponse>> GetEventsByHouseAsync(int houseId)
    {
        return await _context.Events
            .Where(e => e.OrganizingHouseId == houseId)
            .Select(e => e.ToEventResponse())
            .ToListAsync();
    }

    // ✅ Obtener evento por ID
    public async Task<EventResponse?> GetEventByIdAsync(int id)
    {
        var eventEntity = await _context.Events
            .Include(e => e.OrganizingHouse)
            .Include(e => e.TicketTypes)
            .Include(e => e.Products)
            .FirstOrDefaultAsync(e => e.EventId == id);

        return eventEntity?.ToEventResponse();
    }

    // ✅ Crear evento
    public async Task<EventResponse> CreateEventAsync(CreateEventRequest request, int organizingHouseId)
    {
        var eventEntity = request.ToEventEntity();
        eventEntity.OrganizingHouseId = organizingHouseId;
        
        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        return eventEntity.ToEventResponse();
    }

    // ✅ Actualizar evento
    public async Task<EventResponse?> UpdateEventAsync(int id, UpdateEventRequest request)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity == null) return null;

        eventEntity.UpdateEventFromRequest(request);
        await _context.SaveChangesAsync();

        return eventEntity.ToEventResponse();
    }

    // ✅ Actualizar imagen del evento
    public async Task<bool> UpdateEventImageAsync(int eventId, string imageUrl)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null) return false;

        eventEntity.ImageUrl = imageUrl;
        await _context.SaveChangesAsync();
        
        return true;
    }

    // ✅ Eliminar evento
    public async Task<bool> DeleteEventAsync(int id)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Tickets)
            .Include(e => e.Products)
            .FirstOrDefaultAsync(e => e.EventId == id);
        
        if (eventEntity == null) return false;

        // Verificar que no tenga tickets vendidos
        if (eventEntity.Tickets.Any())
            return false;

        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync();
        
        return true;
    }
}