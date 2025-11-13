using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Sale
{
    [Key]
    public int SaleId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty; // Cambiado a string para Identity

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Product? Product { get; set; }
}