namespace Models.DTO;

public class CreateOrganizationRequest
{
    public string Name { get; set; }
}
public class OrganizationMemberDTO
{
    public string Role { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string ProfileImage { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}