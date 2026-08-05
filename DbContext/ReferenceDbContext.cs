using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbContext;

public class ReferenceDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options) { }

    public DbSet<CategoryDbM> Categories { get; set; }
    public DbSet<ProductDbM> Products { get; set; }

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
