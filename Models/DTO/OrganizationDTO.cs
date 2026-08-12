namespace Models.DTO;

public class CreateOrganizationRequest
{
    public string Name { get; set; }
}
public class SelectSubscriptionRequest
{
    public Guid SubscriptionId { get; set; }
}