using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Subscription
{
    [Key]
    public int SubscriptionId { get; set; }

    [ForeignKey("OrganizingHouse")]
    public int OrganizingHouseId { get; set; }

    [Required]
    [StringLength(100)]
    public required string PlanType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public virtual OrganizingHouse? OrganizingHouse { get; set; }
}