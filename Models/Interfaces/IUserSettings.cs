namespace Models;
public interface IUserSettings
{
    Guid Id { get; set; }

    string BannerColor {get; set;}
    bool EmailNotifications { get; set; }
    bool PushNotifications { get; set; }
    bool DarkMode { get; set; }

    string TimeZone {get; set;}
    string Language {get; set;}
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }

    public User User {get; set;}
}