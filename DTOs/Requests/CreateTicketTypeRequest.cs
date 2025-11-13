using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateTicketTypeRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int AvailableQuantity { get; set; }
}