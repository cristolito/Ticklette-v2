using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateEventRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DateTime { get; set; }

    [Required]
    [StringLength(500)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Draft";

    [Required]
    public int OrganizingHouseId { get; set; }

    // ✅ Opción 1: Recibir el archivo directamente (mejor para formularios multipart/form-data)
    public IFormFile? ImageFile { get; set; }
}