using System.ComponentModel.DataAnnotations;

namespace Ticklette.DTOs.Requests;

public class UpdateVirtualCurrencyRequest
{
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; }
}