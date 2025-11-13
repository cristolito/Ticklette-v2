namespace Ticklette.DTOs.Responses;

public class EntryResponse
{
    public int EntryId { get; set; }
    public int TicketId { get; set; }
    public DateTime DateTime { get; set; }
    public string AccessMethod { get; set; } = string.Empty;
}