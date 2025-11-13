using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateOrganizerRequest
{
    // Datos del usuario
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    // Datos del organizador
    [StringLength(255)]
    public string Company { get; set; } = string.Empty;

    [StringLength(20)]
    public string TaxId { get; set; } = string.Empty;

    public string FiscalAddress { get; set; } = string.Empty;

    // Datos de la casa organizadora (obligatoria en registro)
    [Required]
    [StringLength(255)]
    public string OrganizingHouseName { get; set; } = string.Empty;

    [StringLength(500)]
    public string OrganizingHouseAddress { get; set; } = string.Empty;

    [StringLength(255)]
    public string OrganizingHouseContact { get; set; } = string.Empty;

    public string OrganizingHouseTaxData { get; set; } = string.Empty;
}