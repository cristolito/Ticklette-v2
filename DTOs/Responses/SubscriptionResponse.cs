namespace Ticklette.DTOs.Responses;

public class SubscriptionResponse
{
    public int SubscriptionId { get; set; }
    public int OrganizingHouseId { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Cost { get; set; }
    public string Status { get; set; } = string.Empty;
}