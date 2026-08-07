using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbContext;

public class ReferenceDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options) { }

    public DbSet<CategoryDbM> Categories { get; set; }
    public DbSet<ProductDbM> Products { get; set; }
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
        modelBuilder.Entity<CategoryDbM>(entity =>
        {
            entity.HasKey(x => x.CategoryId);
            entity.Property(x => x.CategoryName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.CategorySlug).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<ProductDbM>(entity =>
        {
            entity.HasKey(x => x.ProductId);
            entity.Property(x => x.ProductName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ProductDescription).HasMaxLength(500);
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Passwordhash).IsRequired();
        });

        modelBuilder.Entity<OrganizationDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.HasOne(x => x.Subscription)
                .WithOne(x => x.Organization)
                .HasForeignKey<SubscriptionDbM>(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubscriptionDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StripeCustomerId).HasMaxLength(256).IsRequired();
            entity.Property(x => x.StripeSubscriptionId).HasMaxLength(256).IsRequired();
            entity.HasOne(x => x.Organization)
                .WithOne(x => x.Subscription)
                .HasForeignKey<SubscriptionDbM>(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserOrganizationDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Role).HasMaxLength(120).IsRequired();
            entity.HasOne(x => x.User)
                .WithMany(x => x.Organizations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Organization)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SocialAccountDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Platform).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(120).IsRequired();
            entity.Property(x => x.AccessToken).IsRequired();
            entity.HasOne(x => x.Organization)
                .WithMany(x => x.SocialAccounts)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MediaDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileUrl).HasMaxLength(256).IsRequired();
            entity.Property(x => x.ThumbnailUrl).HasMaxLength(256);
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Duration).HasMaxLength(50);
            entity.HasOne(x => x.Organization)
                .WithMany(x => x.Media)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(120).IsRequired();
            entity.HasOne(x => x.Media)
                .WithMany()
                .HasForeignKey(x => x.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.SocialAccount)
                .WithMany()
                .HasForeignKey(x => x.SocialAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostAnalyticsDbM>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Post)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CategoryDbM>().HasData(
            new CategoryDbM { CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), CategoryName = "Books", CategorySlug = "books" },
            new CategoryDbM { CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), CategoryName = "Games", CategorySlug = "games" }
        );

        modelBuilder.Entity<ProductDbM>().HasData(
            new ProductDbM { ProductId = Guid.Parse("33333333-3333-3333-3333-333333333333"), ProductName = "Clean Architecture Guide", ProductDescription = "A reference book on layered API design.", Price = 199m, IsActive = true, CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
            new ProductDbM { ProductId = Guid.Parse("44444444-4444-4444-4444-444444444444"), ProductName = "Game of Patterns", ProductDescription = "A small sample game product for the reference API.", Price = 349m, IsActive = true, CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222") }
        );

        base.OnModelCreating(modelBuilder);
    }
}
