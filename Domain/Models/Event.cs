using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Event
{
    [Key]
    public int EventId { get; set; }

    [ForeignKey("OrganizingHouse")]
    public int OrganizingHouseId { get; set; }

    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    [Required]
    public required DateTime DateTime { get; set; }

    [Required]
    [StringLength(500)]
    public required string Location { get; set; }

    [Required]
    [StringLength(100)]
    public required string Type { get; set; }

    [Required]
    [StringLength(50)]
    public required string Status { get; set; }

    // ✅ Nueva propiedad para la imagen en Cloudinary
    public string? ImageUrl { get; set; }

    // Navigation properties
    public virtual OrganizingHouse? OrganizingHouse { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
}