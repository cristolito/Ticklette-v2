namespace Ticklette.DTOs.Responses;

public class OrganizingHouseResponse
{
    public int OrganizingHouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string TaxData { get; set; } = string.Empty;
    public int OrganizerId { get; set; }
    public int EventCount { get; set; }
}