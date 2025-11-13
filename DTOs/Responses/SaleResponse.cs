namespace Ticklette.DTOs.Responses;

public class SaleResponse
{
    public int SaleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public ProductResponse Product { get; set; } = new ProductResponse();
}