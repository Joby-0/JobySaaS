namespace Models.DTO;

public class SocialAccountDto
{
    public Guid Id { get; set; }
    public string Platform { get; set; } // "YouTube", "Instagram", etc.
    public string AccountName { get; set; }
    public string ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
}