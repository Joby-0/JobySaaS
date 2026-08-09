using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbContext;

public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
    public DbSet<UserDbM> Users { get; set; }
    public DbSet<OrganizationDbM> Organizations { get; set; }
    public DbSet<SubscriptionDbM> Subscriptions { get; set; }
    public DbSet<UserOrganizationDbM> UserOrganizations { get; set; }
    public DbSet<SocialAccountDbM> SocialAccounts { get; set; }
    public DbSet<PostDbM> Posts { get; set; }
    public DbSet<MediaDbM> Media { get; set; }
    public DbSet<PostAnalyticsDbM> PostAnalytics { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        


        base.OnModelCreating(modelBuilder);
    }
}