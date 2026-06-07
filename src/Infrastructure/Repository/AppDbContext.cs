using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EfProduct> Products { get; set; }
    public DbSet<EfProductGroup> ProductGroups { get; set; }
    public DbSet<EfPriceHistory> PriceHistories { get; set; }
    public DbSet<EfRequest> Requests { get; set; }
    public DbSet<EfUser> Users { get; set; }
    public DbSet<EfProductList> ProductLists { get; set; }
    public DbSet<EfProductListEntry> ProductListEntries { get; set; }
    public DbSet<EfPurchase> Purchases { get; set; }
    public DbSet<EfPurchaseEntry> PurchaseEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfProduct>(eb =>
        {
            eb.Property(e => e.Shop).HasConversion<string>();
            eb.Property(e => e.Unit).HasConversion<string>();
        });
        modelBuilder.Entity<EfRequest>(eb =>
        {
            eb.Property(e => e.SortBy).HasConversion<string>();
            eb.Property(e => e.SortOrder).HasConversion<string>();
        });

        modelBuilder.Entity<EfProductList>(eb =>
        {
            eb.HasOne(x => x.User)
                .WithOne(x => x.ProductList)
                .HasForeignKey<EfProductList>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasMany(x => x.Entries)
                .WithOne(x => x.ProductList)
                .HasForeignKey(x => x.ProductListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EfProductListEntry>(eb =>
        {
            eb.HasOne(x => x.Product)
                .WithMany(x => x.ProductListEntries)
                .HasForeignKey(x => x.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<EfPurchase>(eb =>
        {
            eb.HasOne(x => x.User)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasMany(x => x.Entries)
                .WithOne(x => x.Purchase)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EfPurchaseEntry>(eb =>
        {
            eb.HasOne(x => x.Product)
                .WithMany(x => x.PurchaseEntries)
                .HasForeignKey(x => x.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            eb.Property(x => x.MeasureUnit).HasConversion<string>();

            eb.Property(x => x.Shop).HasConversion<string>();
        });

        // all datetime properties are stored in UTC
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            toDb =>
                toDb.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(toDb, DateTimeKind.Utc)
                    : toDb.ToUniversalTime(),
            fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc)
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (
                var property in entityType
                    .GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
            )
            {
                property.SetValueConverter(utcConverter);
                property.SetColumnType("timestamptz");
            }
        }
    }
}

file static class ModelBuilderExtensions
{
    public static ModelBuilder AddEnumAsTable<TEnum>(this ModelBuilder builder)
        where TEnum : struct, Enum
    {
        var name = typeof(TEnum).Name;
        builder.Entity(name).Property<int>("Id").HasColumnType("integer");
        builder.Entity(name).Property<string>("Value").HasColumnType("text");

        builder.Entity(name).HasKey("Id");
        builder
            .Entity(name)
            .HasData(
                Enum.GetValues<TEnum>()
                    .Select(v => new { Id = Convert.ToInt32(v), Value = v.ToString() })
            );
        return builder;
    }
}
