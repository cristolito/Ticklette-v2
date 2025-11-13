using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class TicketService
{
    private readonly TickletteContext _context;

    public TicketService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Comprar tickets
    public async Task<List<TicketResponse>> PurchaseTicketsAsync(PurchaseTicketRequest request, string userId)
    {
        var ticketType = await _context.TicketTypes.FindAsync(request.TicketTypeId);
        if (ticketType == null || ticketType.AvailableQuantity < request.Quantity)
            throw new InvalidOperationException("Ticket type not available or insufficient quantity");

        var tickets = new List<Ticket>();
        var totalAmount = ticketType.Price * request.Quantity;

        // Verificar saldo en VirtualCurrency
        var virtualCurrency = await _context.VirtualCurrencies
            .FirstOrDefaultAsync(vc => vc.UserId == userId);
        
        if (virtualCurrency == null || virtualCurrency.Balance < totalAmount)
            throw new InvalidOperationException("Insufficient balance");

        // Crear tickets
        for (int i = 0; i < request.Quantity; i++)
        {
            var ticket = new Ticket
            {
                TicketTypeId = request.TicketTypeId,
                UserId = userId,
                Type = ticketType.Name,
                Price = ticketType.Price,
                Status = "Active",
                UniqueCode = GenerateUniqueCode(),
                PurchaseDate = DateTime.UtcNow
            };
            tickets.Add(ticket);
        }

        // Actualizar inventario
        ticketType.AvailableQuantity -= request.Quantity;
        ticketType.SoldQuantity += request.Quantity;

        // Descontar del balance
        virtualCurrency.Balance -= totalAmount;
        virtualCurrency.LastUpdated = DateTime.UtcNow;

        _context.Tickets.AddRange(tickets);
        await _context.SaveChangesAsync();

        return tickets.Select(t => t.ToTicketResponse()).ToList();
    }

    // ✅ Comprar productos
    public async Task<SaleResponse> PurchaseProductAsync(PurchaseProductRequest request, string userId)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null || product.Stock < request.Quantity)
            throw new InvalidOperationException("Product not available or insufficient stock");

        var totalAmount = product.Price * request.Quantity;

        // Verificar saldo en VirtualCurrency
        var virtualCurrency = await _context.VirtualCurrencies
            .FirstOrDefaultAsync(vc => vc.UserId == userId);
        
        if (virtualCurrency == null || virtualCurrency.Balance < totalAmount)
            throw new InvalidOperationException("Insufficient balance");

        // Crear venta
        var sale = new Sale
        {
            UserId = userId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Amount = totalAmount,
            Date = DateTime.UtcNow
        };

        // Actualizar stock
        product.Stock -= request.Quantity;

        // Descontar del balance
        virtualCurrency.Balance -= totalAmount;
        virtualCurrency.LastUpdated = DateTime.UtcNow;

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return sale.ToSaleResponse();
    }

    // ✅ Obtener tickets de un usuario
    public async Task<List<TicketResponse>> GetUserTicketsAsync(string userId)
    {
        return await _context.Tickets
            .Include(t => t.TicketType)
            .ThenInclude(tt => tt!.Event)
            .Where(t => t.UserId == userId)
            .Select(t => t.ToTicketResponse())
            .ToListAsync();
    }

    // ✅ Generar código único para ticket
    private string GenerateUniqueCode()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
    }
}