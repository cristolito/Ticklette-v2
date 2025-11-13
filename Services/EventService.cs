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
    private readonly CloudinaryService _cloudinaryService;

    public EventService(TickletteContext context, CloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
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
        
        // upload image if exists with error handling
        if (request.ImageFile != null)
        {
            var uploadResult = await _cloudinaryService.UploadImageAsync(request.ImageFile, organizingHouseId.ToString());
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                eventEntity.ImageUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                eventEntity.ImageUrl = null;
                // Log the error
                Console.WriteLine($"Cloudinary upload failed: {uploadResult.Error?.Message}");
            }
        }

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        return eventEntity.ToEventResponse();
    }

    // ✅ Actualizar evento
    public async Task<EventResponse?> UpdateEventAsync(int id, UpdateEventRequest request)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity == null) return null;

        // upload image if exists with error handling
        if (request.ImageFile != null)
        {
            var uploadResult = await _cloudinaryService.UploadImageAsync(request.ImageFile, eventEntity.OrganizingHouseId.ToString());
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                eventEntity.ImageUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                // Log the error
                Console.WriteLine($"Cloudinary upload failed: {uploadResult.Error?.Message}");
            }
        }

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

     public async Task<EventResponse?> UploadEventImageAsync(int eventId, IFormFile imageFile)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null)
            return null;

        try
        {
            // Eliminar imagen anterior si existe
            if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
            {
                await DeleteEventImageFromCloudinary(eventEntity.ImageUrl);
            }

            // Subir nueva imagen a Cloudinary
            var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile, eventEntity.OrganizingHouseId.ToString());
            
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");

            // Usar la URL segura proporcionada por Cloudinary
            eventEntity.ImageUrl = uploadResult.SecureUrl.ToString();
            await _context.SaveChangesAsync();

            return eventEntity.ToEventResponse();
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error uploading image: {ex.Message}");
            throw; // Relanzar para que el controlador pueda manejarlo
        }
    }

    // ✅ Eliminar imagen de evento (corregido)
    public async Task<EventResponse?> DeleteEventImageAsync(int eventId)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null || string.IsNullOrEmpty(eventEntity.ImageUrl))
            return null;

        try
        {
            // Eliminar de Cloudinary
            await DeleteEventImageFromCloudinary(eventEntity.ImageUrl);

            // Limpiar URL en la base de datos
            eventEntity.ImageUrl = null;
            await _context.SaveChangesAsync();

            return eventEntity.ToEventResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting image: {ex.Message}");
            throw;
        }
    }

    // ✅ Método helper para eliminar imagen de Cloudinary
    private async Task DeleteEventImageFromCloudinary(string imageUrl)
    {
        try
        {
            var publicId = _cloudinaryService.ExtractPublicIdFromUrl(imageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await _cloudinaryService.DeleteImageAsync(publicId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not delete image from Cloudinary: {ex.Message}");
            // No relanzamos la excepción para no bloquear la operación principal
        }
    }
}