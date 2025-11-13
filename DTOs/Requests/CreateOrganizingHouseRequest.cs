using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateOrganizingHouseRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [StringLength(255)]
    public string Contact { get; set; } = string.Empty;

    public string TaxData { get; set; } = string.Empty;
}