using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Organizer
{
     [Key]
    public int OrganizerId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty; // Cambiado a string para Identity

    [StringLength(255)]
    public string Company { get; set; } = string.Empty;

    [StringLength(20)]
    public string TaxId { get; set; } = string.Empty;

    public string FiscalAddress { get; set; } = string.Empty;

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<OrganizingHouse> OrganizingHouses { get; set; } = new List<OrganizingHouse>();
}