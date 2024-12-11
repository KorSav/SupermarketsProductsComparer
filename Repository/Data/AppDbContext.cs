using Microsoft.EntityFrameworkCore;
using program.Domain;
using program.Domain.Enums;

namespace program.Repository.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Measure> Measures { get; set; }
    public DbSet<ProductStatus> ProductStatuses { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasKey(p => new
            {
                p.Name,
                p.ShopId
            });

        modelBuilder.Entity<Request>()
            .HasKey(r => new
            {
                r.UserId,
                r.Name
            });

        modelBuilder.Entity<Product>()
            .Property(p => p.ShopId)
            .HasConversion<int>();
        modelBuilder.Entity<Shop>()
            .Property(s => s.Id)
            .HasConversion<int>();
        modelBuilder.Entity<Shop>()
            .HasData(GetAllInstancesOf<Shop, ShopId>());

        modelBuilder.Entity<Product>()
            .Property(p => p.MeasureId)
            .HasConversion<int>();
        modelBuilder.Entity<Measure>()
            .Property(m => m.Id)
            .HasConversion<int>();
        modelBuilder.Entity<Measure>()
            .HasData(GetAllInstancesOf<Measure, MeasureId>());

        modelBuilder.Entity<Product>()
            .Property(p => p.ProductStatusId)
            .HasConversion<int>();
        modelBuilder.Entity<ProductStatus>()
            .Property(ps => ps.Id)
            .HasConversion<int>();
        modelBuilder.Entity<ProductStatus>()
            .HasData(GetAllInstancesOf<ProductStatus, ProductStatusId>());
    }

    private static IEnumerable<T> GetAllInstancesOf<T, TEnum>()
        where T : IEnumClass<TEnum>, new()
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
        .Select(e => new T()
        {
            Id = e,
            Name = e.ToString()
        });
    }
}