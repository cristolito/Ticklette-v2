using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class PurchaseProductRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}