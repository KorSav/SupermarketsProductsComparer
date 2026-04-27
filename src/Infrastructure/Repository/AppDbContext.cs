using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(eb =>
        {
            eb.Property(e => e.Shop).HasConversion<string>();
            eb.Property(e => e.Unit).HasConversion<string>();
        });
        modelBuilder.Entity<Request>(eb =>
        {
            eb.Property(e => e.SortById).HasConversion<string>();
            eb.Property(e => e.SortOrderId).HasConversion<string>();
        });
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
