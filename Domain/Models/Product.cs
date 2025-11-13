using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Product
{
    [Key]
    public int ProductId { get; set; }

    [ForeignKey("Event")]
    public int EventId { get; set; }

    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    // Navigation properties
    public virtual Event? Event { get; set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}