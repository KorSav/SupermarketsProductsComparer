using Microsoft.EntityFrameworkCore;
using PriceComparer.Domain;
using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Repository.Data;

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

        modelBuilder.Entity<Request>(eb =>
        {
            string fkUser = "UserId";
            eb.HasOne(r => r.User).WithMany().HasForeignKey(fkUser);
            eb.HasKey(nameof(Request.Name), fkUser);
        });

        User surrogateUser = new()
        {
            Id = -1,
            Name = "admin",
            Email = "admin@gmail.com",
            PasswordHash = "admin",
        };

        modelBuilder.Entity<User>().HasData(surrogateUser);
        string[] requests =
        [
            "м'ясо",
            "сосиски",
            "сардельки",
            "овоч",
            "хліб",
            "фрукт",
            "молоко",
            "сир",
            "йогурт",
            "яйця",
            "риба",
            "крупа",
            "макаронні вироби",
            "цукор",
            "сіль",
            "олія",
            "оцет",
            "кава зерно",
            "кава розчинна",
            "чай",
            "шоколад",
            "печиво",
            "торт",
            "морозиво",
            "сік",
            "вода",
            "напій",
            "ковбаса",
            "консерви",
            "соус",
            "спеції",
            "шампунь",
            "зубна паста",
            "мило",
            "туалетний папір",
            "пральний порошок",
            "корм",
            "підгузки",
        ];
        modelBuilder
            .Entity<Request>()
            .HasData(requests.Select(n => new Request() { Name = n, User = surrogateUser }));
    }
}
