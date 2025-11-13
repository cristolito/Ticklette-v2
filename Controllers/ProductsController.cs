using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ticklette.Domain.Models;
using Ticklette.DTOs.Requests;
using Ticklette.DTOs.Responses;
using Ticklette.Services;

namespace Ticklette.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Organizer")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly EventService _eventService;
    private readonly OrganizingHouseService _houseService;
    private readonly UserManager<User> _userManager;

    public ProductsController(
        ProductService productService,
        EventService eventService,
        OrganizingHouseService houseService,
        UserManager<User> userManager)
    {
        _productService = productService;
        _eventService = eventService;
        _houseService = houseService;
        _userManager = userManager;
    }

    [HttpGet("event/{eventId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByEvent(int eventId)
    {
        var products = await _productService.GetProductsByEventAsync(eventId);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost("event/{eventId}")]
    public async Task<IActionResult> CreateProduct(int eventId, [FromBody] CreateProductRequest request)
    {
        // Verificar permisos sobre el evento
        var eventEntity = await _eventService.GetEventByIdAsync(eventId);
        if (eventEntity == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);

        if (organizerId == null || house?.OrganizerId != organizerId)
            return Forbid();

        var product = await _productService.CreateProductAsync(request, eventId);
        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductRequest request)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        // Verificar permisos
        var eventEntity = await _eventService.GetEventByIdAsync(product.EventId);
        if (eventEntity == null) return NotFound();
        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);
        
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();
        

        if (organizerId == null || house?.OrganizerId != organizerId)
            return Forbid();

        var updatedProduct = await _productService.UpdateProductAsync(id, request);
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        // Verificar permisos
        var eventEntity = await _eventService.GetEventByIdAsync(product.EventId);
        if (eventEntity == null) return NotFound();

        var house = await _houseService.GetHouseByIdAsync(eventEntity.OrganizingHouseId);

        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var organizerId = await _houseService.GetOrganizerIdByUserIdAsync(userId);
        if (organizerId == null) return Forbid();


        if (organizerId == null || house?.OrganizerId != organizerId)
            return Forbid();

        var result = await _productService.DeleteProductAsync(id);
        if (!result) return BadRequest("Cannot delete product with sales");

        return NoContent();
    }
}