using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticklette.Domain.Models;
public class Attendee
{
    [Key]
    public int AttendeeId { get; set; }

    [Required]
    [ForeignKey("User")]
    public required string UserId { get; set; } // Cambiado a string para Identity

    public DateTime DateOfBirth { get; set; }

    [StringLength(50)]
    public string Gender { get; set; } = string.Empty;

    // Navigation properties
    public virtual User? User { get; set; }
}