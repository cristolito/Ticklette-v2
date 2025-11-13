namespace Ticklette.DTOs.Responses;

public class AttendeeResponse
{
    public int AttendeeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public UserResponse User { get; set; } = new UserResponse();
}