using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbContext;

public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
    public DbSet<UserDbM> Users { get; set; }
    public DbSet<OrganizationDbM> Organizations { get; set; }
    public DbSet<SubscriptionPlanDbM> SubscriptionPlans { get; set; }
    public DbSet<OrganizationSubscriptionDbM> OrganizationSubscriptions {get; set;}
    public DbSet<UserOrganizationDbM> UserOrganizations { get; set; }
    public DbSet<SocialAccountDbM> SocialAccounts { get; set; }
    public DbSet<PostDbM> Posts { get; set; }
    public DbSet<MediaDbM> Media { get; set; }
    public DbSet<PostAnalyticsDbM> PostAnalytics { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubscriptionPlanDbM>().HasData(
        new SubscriptionPlanDbM
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Free",
            Price = 0,
            StripePriceId = null,
        },
        new SubscriptionPlanDbM
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Pro Monthly",
            Price = 10,
            StripePriceId = "price_1U3MahBD6lDY7COkvKIK29cg",
            BillingIntervalInMonths = 1
        },
        new SubscriptionPlanDbM
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Business Monthly",
            Price = 30,
            StripePriceId = "price_1U3MgCBD6lDY7COkr8xmg0Ha",
            BillingIntervalInMonths = 1
            
        },
        new SubscriptionPlanDbM
        {
            Id = Guid.Parse("price_1U3N6rBD6lDY7COkO6ixkp2H"),
            Name = "Pro Yearly",
            Price = 100,
            StripePriceId = "prod_V3TYdFVSK3pCoo",
            BillingIntervalInMonths = 12
        },
        new SubscriptionPlanDbM
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Name = "Business Yearly",
            Price = 300,
            StripePriceId = "price_1U3N6BBD6lDY7COkF4N9qk3K",
            BillingIntervalInMonths = 12
        }
    );
    }
}