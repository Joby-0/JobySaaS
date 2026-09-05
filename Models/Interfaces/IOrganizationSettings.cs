namespace Models;

public interface IOrganizationSettings
{
    Guid Id { get; set; }


    string TimeZone { get; set; }
    string DefaultLanguage { get; set; }

    bool RequireApprovalBeforePublishing { get; set; }
    bool EnableAnalytics { get; set; }

    public Organization Organization { get; set; }
}