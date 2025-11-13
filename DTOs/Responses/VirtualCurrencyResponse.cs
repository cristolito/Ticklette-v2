namespace Ticklette.DTOs.Responses;

public class VirtualCurrencyResponse
{
    public int VirtualCurrencyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
}