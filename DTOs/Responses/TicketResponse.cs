namespace Ticklette.DTOs.Responses;

public class TicketResponse
{
    public int TicketId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UniqueCode { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int TicketTypeId { get; set; }
    public EntryResponse? Entry { get; set; }
}