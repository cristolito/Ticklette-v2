using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Ticklette.Domain.Models;
public class User : IdentityUser
{
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    // Rol personalizado adicional si lo necesitas
    [Range(0, 1)]
    public int CustomRole { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
     // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual VirtualCurrency? VirtualCurrency { get; set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual Organizer? Organizer { get; set; }
    public virtual Attendee? Attendee { get; set; }
}