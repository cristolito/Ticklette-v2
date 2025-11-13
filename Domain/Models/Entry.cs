using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Entry
{
    [Key]
    public int EntryId { get; set; }

    [ForeignKey("Ticket")]
    public int TicketId { get; set; }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(100)]
    public required string AccessMethod { get; set; }

    // Navigation properties
    public virtual Ticket? Ticket { get; set; }
}