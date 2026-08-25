using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbContext;

public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
    public DbSet<UserDbM> Users { get; set; }
    public DbSet<OrganizationDbM> Organizations { get; set; }
    public DbSet<SubscriptionPlanDbM> SubscriptionPlans { get; set; }
    public DbSet<OrganizationSubscriptionDbM> OrganizationSubscriptions { get; set; }
    public DbSet<UserOrganizationDbM> UserOrganizations { get; set; }
    public DbSet<SocialAccountDbM> SocialAccounts { get; set; }
    public DbSet<PostDbM> Posts { get; set; }
    public DbSet<MediaDbM> Media { get; set; }
    public DbSet<PostAnalyticsDbM> PostAnalytics { get; set; }
    public DbSet<OrganizationInvitationDbM> OrganizationInvitations { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Organization invitations
        modelBuilder.Entity<OrganizationInvitationDbM>()
            .HasIndex(x => x.InviteCode)
            .IsUnique();


        // --------------------------------------------------
        // Subscription Plans
        // --------------------------------------------------

        modelBuilder.Entity<SubscriptionPlanDbM>().HasData(

            // FREE
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Free",
                Price = 0,
                StripePriceId = null,
                BillingIntervalInMonths = 1,
                IsActive = true,
                isFree = true,
                ContactSales = false
            },

            // BASIC MONTHLY
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Basic",
                Price = 10,
                StripePriceId = "YOUR_BASIC_MONTHLY_PRICE_ID",
                BillingIntervalInMonths = 1,
                IsActive = true,
                isFree = false,
                ContactSales = false
            },

            // PRO MONTHLY
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Pro",
                Price = 25,
                StripePriceId = "YOUR_PRO_MONTHLY_PRICE_ID",
                BillingIntervalInMonths = 1,
                IsActive = true,
                isFree = false,
                ContactSales = false
            },

            // ENTERPRISE
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Enterprise",
                Price = 0,
                StripePriceId = null,
                BillingIntervalInMonths = 1,
                ContactSales = true,
                IsActive = true,
                isFree = false
            },

            // BASIC YEARLY
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Basic",
                Price = 100,
                StripePriceId = "YOUR_BASIC_YEARLY_PRICE_ID",
                BillingIntervalInMonths = 12,
                IsActive = true,
                isFree = false,
                ContactSales = false
            },

            // PRO YEARLY
            new SubscriptionPlanDbM
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Pro",
                Price = 250,
                StripePriceId = "YOUR_PRO_YEARLY_PRICE_ID",
                BillingIntervalInMonths = 12,
                IsActive = true,
                isFree = false,
                ContactSales = false
            }
        );


        // --------------------------------------------------
        // Features
        // --------------------------------------------------

        modelBuilder.Entity<FeatureDbM>().HasData(

            // ==================================================
            // FREE
            // ==================================================

            new FeatureDbM
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "1 user",
                SubscriptionPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab"),
                Name = "1 social account",
                SubscriptionPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac"),
                Name = "Limited uploads",
                SubscriptionPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad"),
                Name = "Basic analytics",
                SubscriptionPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },


            // ==================================================
            // BASIC
            // ==================================================

            new FeatureDbM
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbaa"),
                Name = "Up to 5 users",
                SubscriptionPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbab"),
                Name = "Up to 5 social accounts",
                SubscriptionPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbac"),
                Name = "Unlimited uploads",
                SubscriptionPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbad"),
                Name = "Basic analytics",
                SubscriptionPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbae"),
                Name = "Scheduled publishing",
                SubscriptionPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },


            // ==================================================
            // PRO
            // ==================================================

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccca"),
                Name = "Up to 15 users",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccb"),
                Name = "Up to 20 social accounts",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Publish to X (Twitter)",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccd"),
                Name = "Unlimited uploads",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccce"),
                Name = "Advanced analytics",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccf"),
                Name = "Scheduled publishing",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccd0"),
                Name = "Content management",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccd1"),
                Name = "Team collaboration",
                SubscriptionPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },


            // ==================================================
            // ENTERPRISE
            // ==================================================

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddda"),
                Name = "Unlimited users",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddb"),
                Name = "Unlimited social accounts",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddc"),
                Name = "Publish to X (Twitter)",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "Unlimited uploads",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddde"),
                Name = "Advanced analytics",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddf"),
                Name = "Scheduled publishing",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddf0"),
                Name = "Team collaboration",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddf1"),
                Name = "Priority support",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddf2"),
                Name = "Custom integrations",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            },

            new FeatureDbM
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddf3"),
                Name = "Dedicated support",
                SubscriptionPlanId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            }
        );
    }
}