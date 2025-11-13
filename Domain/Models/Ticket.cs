using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Ticket
{
    [Key]
    public int TicketId { get; set; }

    [ForeignKey("TicketType")]
    public int TicketTypeId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty; // Cambiado a string para Identity

    [StringLength(50)]
    public required string Type { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(50)]
    public required string Status { get; set; }

    [Required]
    [StringLength(100)]
    public required string UniqueCode { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual TicketType? TicketType { get; set; }
    public virtual User? User { get; set; }
    public virtual Entry? Entry { get; set; }
}