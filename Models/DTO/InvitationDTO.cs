namespace Models.DTO;

public class InvitationPreviewDto
{
    public string Organization {get; set;}
    public string InvitedBy {get; set;}
    public DateTime ExpireAt {get; set;}
}