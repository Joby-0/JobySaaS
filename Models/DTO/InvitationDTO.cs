namespace Models.DTO;

public class InvitationPreviewDto
{
    public string Organization { get; set; }
    public string InvitedBy { get; set; }
    public DateTime ExpireAt { get; set; }
}


public class InvitationUpdate
{
    public Guid Id { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? AcceptedAt { get; set; }

}