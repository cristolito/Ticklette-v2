namespace Ticklette.DTOs.Responses;

public class EventResponse
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int OrganizingHouseId { get; set; }
    public DateTime CreatedAt { get; set; }
}