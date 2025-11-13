using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class UpdateEventRequest
{
    public int EventId { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? DateTime { get; set; }

    [StringLength(500)]
    public string? Location { get; set; }

    [StringLength(100)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public int? OrganizingHouseId { get; set; }

    // Campos para imagen (opcionales en update)
    public IFormFile? ImageFile { get; set; }
}