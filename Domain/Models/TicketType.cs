using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class TicketType
{
    [Key]
    public int TicketTypeId { get; set; }

    [ForeignKey("Event")]
    public int EventId { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int AvailableQuantity { get; set; }

    public int SoldQuantity { get; set; }

    // Navigation properties
    public virtual Event? Event { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}