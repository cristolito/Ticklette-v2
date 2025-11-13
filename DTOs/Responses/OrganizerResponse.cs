namespace Ticklette.DTOs.Responses;

public class OrganizerResponse
{
    public int OrganizerId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string FiscalAddress { get; set; } = string.Empty;
    public UserResponse User { get; set; } = new UserResponse();
    public List<OrganizingHouseResponse> OrganizingHouses { get; set; } = new List<OrganizingHouseResponse>();
}