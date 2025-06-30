using Microsoft.EntityFrameworkCore;
using program.Domain;
using program.Domain.Enums;

namespace program.Repository.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .AddEnumLookup<Measure>((typeof(Product), nameof(Product.Measure)))
            .AddEnumLookup<ProductStatus>((typeof(Product), nameof(Product.ProductStatus)))
            .AddEnumLookup<Shop>((typeof(Product), nameof(Product.Shop)))
            .AddEnumLookup<SortBy>((typeof(Request), nameof(Request.Sort)))
            .AddEnumLookup<SortOrder>((typeof(Request), nameof(Request.SortOrder)));

        modelBuilder.Entity<Request>().HasKey(e => new { e.Name, e.UserId });

        modelBuilder
            .Entity<User>()
            .HasData(
                [
                    new()
                    {
                        Id = -1,
                        Name = "admin",
                        Email = "admin@gmail.com",
                        PasswordHash = "admin",
                    },
                ]
            );
        modelBuilder
            .Entity<Request>()
            .HasData(
                [
                    new() { Name = "м'ясо", UserId = -1 },
                    new() { Name = "сосиски", UserId = -1 },
                    new() { Name = "сардельки", UserId = -1 },
                    new() { Name = "овоч", UserId = -1 },
                    new() { Name = "хліб", UserId = -1 },
                    new() { Name = "фрукт", UserId = -1 },
                    new() { Name = "молоко", UserId = -1 },
                    new() { Name = "сир", UserId = -1 },
                    new() { Name = "йогурт", UserId = -1 },
                    new() { Name = "яйця", UserId = -1 },
                    new() { Name = "риба", UserId = -1 },
                    new() { Name = "крупа", UserId = -1 },
                    new() { Name = "макаронні вироби", UserId = -1 },
                    new() { Name = "цукор", UserId = -1 },
                    new() { Name = "сіль", UserId = -1 },
                    new() { Name = "олія", UserId = -1 },
                    new() { Name = "оцет", UserId = -1 },
                    new() { Name = "кава зерно", UserId = -1 },
                    new() { Name = "кава розчинна", UserId = -1 },
                    new() { Name = "чай", UserId = -1 },
                    new() { Name = "шоколад", UserId = -1 },
                    new() { Name = "печиво", UserId = -1 },
                    new() { Name = "торт", UserId = -1 },
                    new() { Name = "морозиво", UserId = -1 },
                    new() { Name = "сік", UserId = -1 },
                    new() { Name = "вода", UserId = -1 },
                    new() { Name = "напій", UserId = -1 },
                    new() { Name = "ковбаса", UserId = -1 },
                    new() { Name = "консерви", UserId = -1 },
                    new() { Name = "соус", UserId = -1 },
                    new() { Name = "спеції", UserId = -1 },
                    new() { Name = "шампунь", UserId = -1 },
                    new() { Name = "зубна паста", UserId = -1 },
                    new() { Name = "мило", UserId = -1 },
                    new() { Name = "туалетний папір", UserId = -1 },
                    new() { Name = "пральний порошок", UserId = -1 },
                    new() { Name = "корм", UserId = -1 },
                    new() { Name = "підгузки", UserId = -1 },
                ]
            );
    }
}
