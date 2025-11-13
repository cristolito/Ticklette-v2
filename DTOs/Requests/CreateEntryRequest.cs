using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class CreateEntryRequest
{
    [Required]
    [StringLength(100)]
    public string AccessMethod { get; set; } = "QR"; // QR, Manual, NFC, etc.

    [Required]
    public int TicketId { get; set; }
}