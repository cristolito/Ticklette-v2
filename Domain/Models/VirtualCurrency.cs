using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class VirtualCurrency
{
    [Key]
    public int VirtualCurrencyId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty; // Cambiado a string para Identity

    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }
}