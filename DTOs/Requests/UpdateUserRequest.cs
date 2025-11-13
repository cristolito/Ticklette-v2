using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class UpdateUserRequest
{
    public string UserId { get; set; } = string.Empty;
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

    [Range(0, 1)]
    public int CustomRole { get; set; } = 0;
}