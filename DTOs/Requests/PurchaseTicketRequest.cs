using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class PurchaseTicketRequest
{
    [Required]
    public int TicketTypeId { get; set; }

    [Required]
    [Range(1, 10)] // Máximo 10 tickets por compra
    public int Quantity { get; set; } = 1;
}