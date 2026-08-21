using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class OrganizationInvitationDbM : OrganizationInvitation
{
    [Key]
    public override Guid Id { get; set; }

    public override Guid InvitedByUserId { get; set; }
    public override Guid OrganizationId { get; set; }
}
