using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class VirtualCurrencyService
{
    private readonly TickletteContext _context;

    public VirtualCurrencyService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Obtener balance de usuario
    public async Task<VirtualCurrencyResponse?> GetBalanceAsync(string userId)
    {
        var virtualCurrency = await _context.VirtualCurrencies
            .FirstOrDefaultAsync(vc => vc.UserId == userId);
        
        return virtualCurrency?.ToVirtualCurrencyResponse();
    }

    // ✅ Actualizar balance (para administradores)
    public async Task<VirtualCurrencyResponse?> UpdateBalanceAsync(string userId, UpdateVirtualCurrencyRequest request)
    {
        var virtualCurrency = await _context.VirtualCurrencies
            .FirstOrDefaultAsync(vc => vc.UserId == userId);
        
        if (virtualCurrency == null) return null;

        virtualCurrency.Balance = request.Balance;
        virtualCurrency.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return virtualCurrency.ToVirtualCurrencyResponse();
    }

    // ✅ Recargar balance
    public async Task<VirtualCurrencyResponse?> AddBalanceAsync(string userId, decimal amount)
    {
        if (amount <= 0) return null;

        var virtualCurrency = await _context.VirtualCurrencies
            .FirstOrDefaultAsync(vc => vc.UserId == userId);
        
        if (virtualCurrency == null) return null;

        virtualCurrency.Balance += amount;
        virtualCurrency.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return virtualCurrency.ToVirtualCurrencyResponse();
    }
}