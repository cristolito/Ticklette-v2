namespace Ticklette.DTOs.Responses;

public class TicketTypeResponse
{
    public int TicketTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public int EventId { get; set; }
}