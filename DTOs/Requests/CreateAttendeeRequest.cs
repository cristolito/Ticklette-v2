using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateAttendeeRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(50)]
    public string Gender { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
}