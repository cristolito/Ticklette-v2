namespace Ticklette.DTOs.Responses;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int CustomRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
}