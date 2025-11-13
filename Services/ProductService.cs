using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Data;
using Ticklette.DTOs.Mappers;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;

namespace Ticklette.Services;

public class ProductService
{
    private readonly TickletteContext _context;

    public ProductService(TickletteContext context)
    {
        _context = context;
    }

    // ✅ Obtener productos de un evento
    public async Task<List<ProductResponse>> GetProductsByEventAsync(int eventId)
    {
        return await _context.Products
            .Where(p => p.EventId == eventId)
            .Select(p => p.ToProductResponse())
            .ToListAsync();
    }

    // ✅ Obtener producto por ID
    public async Task<ProductResponse?> GetProductByIdAsync(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == productId);
        
        return product?.ToProductResponse();
    }

    // ✅ Crear producto
    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request, int eventId)
    {
        var product = request.ToProductEntity(eventId);
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.ToProductResponse();
    }

    // ✅ Actualizar producto
    public async Task<ProductResponse?> UpdateProductAsync(int productId, CreateProductRequest request)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await _context.SaveChangesAsync();
        return product.ToProductResponse();
    }

    // ✅ Eliminar producto
    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Sales)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
        
        if (product == null || product.Sales.Any())
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}