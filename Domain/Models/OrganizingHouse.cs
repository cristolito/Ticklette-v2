using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class OrganizingHouse
{
    [Key]
    public int OrganizingHouseId { get; set; }

    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [StringLength(255)]
    public string Contact { get; set; } = string.Empty;

    public string TaxData { get; set; } = string.Empty;

    [ForeignKey("Organizer")]
    public int OrganizerId { get; set; }

    // Navigation properties
    public virtual Organizer? Organizer { get; set; }
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}